public sealed class Age : IEquatable<Age>
{
    public int Value { get; set; }

    public Age(int value)
    {

        if(value < 0) throw new ArgumentOutOfRangeException("Yaş negatif olamaz");
        if(value > 150) throw new ArgumentOutOfRangeException("Yaş 150'den büyük olamaz");

        Value = value;
    }

    public bool IsAdult => Value >= 18;
    public bool IsSenior => Value >= 60;


    public Age NextBirthDay() => new Age(Value+1);


    public bool Equals(Age? other) => other is not null && Value == other.Value;


}