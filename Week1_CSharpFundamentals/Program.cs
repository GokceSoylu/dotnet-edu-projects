using System.Net;
using Week1_CSharpFundamentals; // Modellerimizi buraya çağırıyoruz


CustomerInfo customerInfo = new CustomerInfo(0, "Ahmet HMDİ", 52);

Customer customer = new Customer("HASAN SABBAH", customerInfo);
Console.WriteLine(customerInfo);

String result = customerInfo.Id switch
{
    200 => "OK",
    404 => "NOT FOUND",
    0 => "First Customer",
    _ => "Unknown"
};

Console.WriteLine(result);