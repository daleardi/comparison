public class DiscountManager
{
  private readonly IAccountDiscountCalculatorFactory _factory;
  private readonly ILoyaltyDiscountCalculator _loyaltyDiscountCalculator;
 
  public DiscountManager(IAccountDiscountCalculatorFactory factory, ILoyaltyDiscountCalculator loyaltyDiscountCalculator)
  {
    _factory = factory;
    _loyaltyDiscountCalculator = loyaltyDiscountCalculator;
  }
 
  public decimal AddDiscount(decimal price, AccountStatus accountStatus, int timeOfHavingAccountInYears)
  {
    decimal priceAfterDiscount = 0;
    priceAfterDiscount = _factory.GetAccountDiscountCalculator(accountStatus).AddDiscount(price);
    priceAfterDiscount = _loyaltyDiscountCalculator.AddDiscount(priceAfterDiscount, timeOfHavingAccountInYears);
    return priceAfterDiscount;
  }
}

public static class Constants
{
  public const int MAXIMUM_DISCOUNT_FOR_LOYALTY = 5;
  public const decimal DISCOUNT_FOR_SIMPLE_CUSTOMERS = 0.1m;
  public const decimal DISCOUNT_FOR_VALUABLE_CUSTOMERS = 0.3m;
  public const decimal DISCOUNT_FOR_MOST_VALUABLE_CUSTOMERS = 0.5m;
}

public static class PriceExtensions
{
  public static decimal AddDiscountForAccountStatus(this decimal price, decimal discountSize)
  {
    return price - (discountSize * price);
  }
 
  public static decimal AddDiscountForTimeOfHavingAccount(this decimal price, int timeOfHavingAccountInYears)
  {
     decimal discountForLoyaltyInPercentage = (timeOfHavingAccountInYears > Constants.MAXIMUM_DISCOUNT_FOR_LOYALTY) ? (decimal)Constants.MAXIMUM_DISCOUNT_FOR_LOYALTY/100 : (decimal)timeOfHavingAccountInYears/100;
    return price - (discountForLoyaltyInPercentage * price);
  }
}

public interface ILoyaltyDiscountCalculator
{
  decimal AddDiscount(decimal price, int timeOfHavingAccountInYears);
}
 
public class DefaultLoyaltyDiscountCalculator : ILoyaltyDiscountCalculator
{
  public decimal AddDiscount(decimal price, int timeOfHavingAccountInYears)
  {
    decimal discountForLoyaltyInPercentage = (timeOfHavingAccountInYears > Constants.MAXIMUM_DISCOUNT_FOR_LOYALTY) ? (decimal)Constants.MAXIMUM_DISCOUNT_FOR_LOYALTY/100 : (decimal)timeOfHavingAccountInYears/100;
    return price - (discountForLoyaltyInPercentage * price);
  }
}

public interface IAccountDiscountCalculatorFactory
{
  IAccountDiscountCalculator GetAccountDiscountCalculator(AccountStatus accountStatus);
}
 
public class DefaultAccountDiscountCalculatorFactory : IAccountDiscountCalculatorFactory
{
  public IAccountDiscountCalculator GetAccountDiscountCalculator(AccountStatus accountStatus)
  {
    IAccountDiscountCalculator calculator;
    switch (accountStatus)
    {
      case AccountStatus.NotRegistered:
        calculator = new NotRegisteredDiscountCalculator();
        break;
      case AccountStatus.SimpleCustomer:
        calculator = new SimpleCustomerDiscountCalculator();
        break;
      case AccountStatus.ValuableCustomer:
        calculator = new ValuableCustomerDiscountCalculator();
        break;
      case AccountStatus.MostValuableCustomer:
        calculator = new MostValuableCustomerDiscountCalculator();
        break;
      default:
        throw new NotImplementedException();
    }
 
    return calculator;
  }
}
Hide   Shrink    Copy Code
public interface IAccountDiscountCalculator
{
  decimal AddDiscount(decimal price);
}
 
public class NotRegisteredDiscountCalculator : IAccountDiscountCalculator
{
  public decimal AddDiscount(decimal price)
  {
    return price;
  }
}
 
public class SimpleCustomerDiscountCalculator : IAccountDiscountCalculator
{
  public decimal AddDiscount(decimal price)
  {
    return price - (Constants.DISCOUNT_FOR_SIMPLE_CUSTOMERS * price);
  }
}
 
public class ValuableCustomerDiscountCalculator : IAccountDiscountCalculator
{
  public decimal AddDiscount(decimal price)
  {
    return price - (Constants.DISCOUNT_FOR_VALUABLE_CUSTOMERS * price);
  }
}
 
public class MostValuableCustomerDiscountCalculator : IAccountDiscountCalculator
{
  public decimal AddDiscount(decimal price)
  {
    return price - (Constants.DISCOUNT_FOR_MOST_VALUABLE_CUSTOMERS * price);
  }
}

