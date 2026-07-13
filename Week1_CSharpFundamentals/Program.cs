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

int Purchase = customerInfo.Age switch
{
    > 0 and < 18 => 0,
    >= 18 and < 65 => 15,
    > 65 => 0,
    _ => 404
};
Console.WriteLine(Purchase);

String nfo = customer switch
{
    { info.Age: 15 } => "Ekrem",
    { agent: "Hasan Sabbah", info.Id: 0 } => "yey",
    _ => "non"

};
Console.WriteLine(nfo);

int[] num = { 1, 2, 3, 4, 5, 6 };
bool match = num switch
{
    [1, 2, _, _] => true,
    [_, _, _, _, 10] => true,
    [1, 2, 3, _, _, 6] => true,
    _ => false
};

Console.WriteLine(match);