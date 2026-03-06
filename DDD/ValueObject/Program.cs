// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var ali = new User("Ali","Ali123",new Age(18));


Console.WriteLine(ali.Age.IsAdult);
Console.WriteLine(ali.Age.NextBirthDay().Value);
Console.WriteLine(ali.Age.Equals(new Age(18)));