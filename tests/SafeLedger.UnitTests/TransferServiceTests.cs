using Microsoft.Extensions.Logging;
using Moq;
using SafeLedger.Application.DTOs;
using SafeLedger.Application.Interfaces;
using SafeLedger.Application.Services;
using SafeLedger.Domain.Entities;
using SafeLedger.Domain.Interfaces;

namespace SafeLedger.UnitTests;

/// <summary>
/// Tests unitarios para TransferService enfocados en validar la lógica de transferencias financieras.
/// </summary>
public class TransferServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IFraudDetectionService> _fraudDetectionServiceMock;
    private readonly Mock<ILogger<TransferService>> _loggerMock;
    private readonly Mock<ITransaction> _transactionMock;
    private readonly TransferService _transferService;

    public TransferServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _fraudDetectionServiceMock = new Mock<IFraudDetectionService>();
        _loggerMock = new Mock<ILogger<TransferService>>();
        _transactionMock = new Mock<ITransaction>();

        _unitOfWorkMock.Setup(u => u.Accounts).Returns(_accountRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Transactions).Returns(_transactionRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _transferService = new TransferService(
            _unitOfWorkMock.Object,
            _fraudDetectionServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ExecuteTransferAsync_TransferenciaExitosa_GuardaRegistroTransaccion()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var amount = 1000.50m;
        var request = new TransferRequestDto(senderId, receiverId, amount, "Test transfer");

        var sender = new Account(senderId, Guid.NewGuid(), "USD", 5000m);
        var receiver = new Account(receiverId, Guid.NewGuid(), "USD", 1000m);

        _fraudDetectionServiceMock
            .Setup(f => f.IsSuspiciousAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _accountRepositoryMock
            .Setup(r => r.GetByIdWithLockAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sender);

        _accountRepositoryMock
            .Setup(r => r.GetByIdWithLockAsync(receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receiver);

        TransactionRecord? savedTransaction = null;
        _transactionRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TransactionRecord>(), It.IsAny<CancellationToken>()))
            .Callback<TransactionRecord, CancellationToken>((t, ct) => savedTransaction = t)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transferService.ExecuteTransferAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(savedTransaction);
        Assert.Equal(senderId, savedTransaction.SenderId);
        Assert.Equal(receiverId, savedTransaction.ReceiverId);
        Assert.Equal(amount, savedTransaction.Amount);
        Assert.Equal("Test transfer", savedTransaction.Reference);
        Assert.Equal(5000m - amount, sender.Balance); // 5000 - 1000.50 = 3999.50
        Assert.Equal(1000m + amount, receiver.Balance); // 1000 + 1000.50 = 2000.50

        _transactionRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TransactionRecord>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteTransferAsync_FondosInsuficientes_LanzaExcepcion()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var amount = 10000m;
        var request = new TransferRequestDto(senderId, receiverId, amount, "Test transfer");

        var sender = new Account(senderId, Guid.NewGuid(), "USD", 1000m); // Fondos insuficientes
        var receiver = new Account(receiverId, Guid.NewGuid(), "USD", 0m);

        _fraudDetectionServiceMock
            .Setup(f => f.IsSuspiciousAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _accountRepositoryMock
            .Setup(r => r.GetByIdWithLockAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sender);

        _accountRepositoryMock
            .Setup(r => r.GetByIdWithLockAsync(receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receiver);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _transferService.ExecuteTransferAsync(request, CancellationToken.None));

        _transactionRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TransactionRecord>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteTransferAsync_CuentaOrigenNoEncontrada_LanzaExcepcion()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var amount = 1000m;
        var request = new TransferRequestDto(senderId, receiverId, amount, "Test transfer");

        _fraudDetectionServiceMock
            .Setup(f => f.IsSuspiciousAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _accountRepositoryMock
            .Setup(r => r.GetByIdWithLockAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _transferService.ExecuteTransferAsync(request, CancellationToken.None));

        Assert.Contains(senderId.ToString(), exception.Message);
        _transactionRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TransactionRecord>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteTransferAsync_CuentaDestinoNoEncontrada_LanzaExcepcion()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var amount = 1000m;
        var request = new TransferRequestDto(senderId, receiverId, amount, "Test transfer");

        var sender = new Account(senderId, Guid.NewGuid(), "USD", 5000m);

        _fraudDetectionServiceMock
            .Setup(f => f.IsSuspiciousAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _accountRepositoryMock
            .Setup(r => r.GetByIdWithLockAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sender);

        _accountRepositoryMock
            .Setup(r => r.GetByIdWithLockAsync(receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _transferService.ExecuteTransferAsync(request, CancellationToken.None));

        Assert.Contains(receiverId.ToString(), exception.Message);
        _transactionRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TransactionRecord>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteTransferAsync_FraudeDetectado_LanzaExcepcionNoAutorizada()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var amount = 60000m; // Monto sospechoso (mayor a 50000)
        var request = new TransferRequestDto(senderId, receiverId, amount, "Test transfer");

        _fraudDetectionServiceMock
            .Setup(f => f.IsSuspiciousAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _transferService.ExecuteTransferAsync(request, CancellationToken.None));

        _accountRepositoryMock.Verify(
            r => r.GetByIdWithLockAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _transactionRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TransactionRecord>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteTransferAsync_ErrorEnGuardado_HaceRollback()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var amount = 1000m;
        var request = new TransferRequestDto(senderId, receiverId, amount, "Test transfer");

        var sender = new Account(senderId, Guid.NewGuid(), "USD", 5000m);
        var receiver = new Account(receiverId, Guid.NewGuid(), "USD", 1000m);

        _fraudDetectionServiceMock
            .Setup(f => f.IsSuspiciousAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _accountRepositoryMock
            .Setup(r => r.GetByIdWithLockAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sender);

        _accountRepositoryMock
            .Setup(r => r.GetByIdWithLockAsync(receiverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receiver);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _transferService.ExecuteTransferAsync(request, CancellationToken.None));

        _transactionRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TransactionRecord>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

