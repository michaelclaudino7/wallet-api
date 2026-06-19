namespace WalletApi.Services;

public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string message) : base(message) { }
}

public class AccountNotFoundException : Exception
{
    public AccountNotFoundException(string message) : base(message) { }
}

public class InvalidAmountException : Exception
{
    public InvalidAmountException(string message) : base(message) { }
}
