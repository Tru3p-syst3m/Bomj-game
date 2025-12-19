using Xunit;
using valera3.Models;

public class ValeraTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        var valera = new Valera();

        Assert.Equal(100, valera.Health);
        Assert.Equal(0, valera.Mana);
        Assert.Equal(0, valera.Cheerfulness);
        Assert.Equal(0, valera.Fatigue);
        Assert.Equal(100, valera.Money);
        Assert.True(valera.IsAlive);
    }

    [Fact]
    public void Constructor_InitializesWithCustomValues()
    {
        var valera = new Valera(80, 20, 5, 30, 200);

        Assert.Equal(80, valera.Health);
        Assert.Equal(20, valera.Mana);
        Assert.Equal(5, valera.Cheerfulness);
        Assert.Equal(30, valera.Fatigue);
        Assert.Equal(200, valera.Money);
    }

    [Fact]
    public void GoToWork_Success_WhenConditionsMet()
    {
        var valera = new Valera(mana: 40, fatigue : 5, money : 50);

        var result = valera.GoToWork();

        Assert.True(result);
        Assert.Equal(-5, valera.Cheerfulness); 
        Assert.Equal(10, valera.Mana); 
        Assert.Equal(150, valera.Money);
        Assert.Equal(75, valera.Fatigue);
    }

    [Fact]
    public void GoToWork_Fails_WhenManaTooHigh()
    {
        var valera = new Valera(mana: 50, fatigue : 5);

        var result = valera.GoToWork();

        Assert.False(result);
        Assert.Equal(0, valera.Cheerfulness);
        Assert.Equal(50, valera.Mana);
        Assert.Equal(100, valera.Money);
        Assert.Equal(5, valera.Fatigue);
    }

    [Fact]
    public void GoToWork_Fails_WhenFatigueTooHigh()
    {
        var valera = new Valera(mana: 40, fatigue : 10);

        var result = valera.GoToWork();

        Assert.False(result);
        Assert.Equal(0, valera.Cheerfulness);
        Assert.Equal(40, valera.Mana);
        Assert.Equal(100, valera.Money);
        Assert.Equal(10, valera.Fatigue);
    }

    [Fact]
    public void ContemplateNature_ChangesStateCorrectly()
    {
        var valera = new Valera(mana: 50, fatigue : 20);

        valera.ContemplateNature();

        Assert.Equal(1, valera.Cheerfulness);
        Assert.Equal(40, valera.Mana); // 50 - 10
        Assert.Equal(30, valera.Fatigue); // 20 + 10
    }

    [Fact]
    public void DrinkWineAndWatchSeries_Success_WhenEnoughMoney()
    {
        var valera = new Valera(money: 50);

        var result = valera.DrinkWineAndWatchSeries();

        Assert.True(result);
        Assert.Equal(-1, valera.Cheerfulness);
        Assert.Equal(30, valera.Mana);
        Assert.Equal(10, valera.Fatigue);
        Assert.Equal(95, valera.Health); // 100 - 5
        Assert.Equal(30, valera.Money); // 50 - 20
    }

    [Fact]
    public void DrinkWineAndWatchSeries_Fails_WhenNotEnoughMoney()
    {
        var valera = new Valera(money: 10);

        var result = valera.DrinkWineAndWatchSeries();

        Assert.False(result);
        Assert.Equal(0, valera.Cheerfulness);
        Assert.Equal(0, valera.Mana);
        Assert.Equal(0, valera.Fatigue);
        Assert.Equal(100, valera.Health);
        Assert.Equal(10, valera.Money);
    }

    [Fact]
    public void GoToBar_Success_WhenEnoughMoney()
    {
        var valera = new Valera(money: 150);

        var result = valera.GoToBar();
        
        Assert.True(result);
        Assert.Equal(1, valera.Cheerfulness);
        Assert.Equal(60, valera.Mana);
        Assert.Equal(40, valera.Fatigue);
        Assert.Equal(90, valera.Health); // 100 - 10
        Assert.Equal(50, valera.Money); // 150 - 100
    }

    [Fact]
    public void DrinkWithMarginals_Success_WhenEnoughMoney()
    {
        var valera = new Valera(money: 200);

        var result = valera.DrinkWithMarginals();

        Assert.True(result);
        Assert.Equal(5, valera.Cheerfulness);
        Assert.Equal(20, valera.Health); // 100 - 80
        Assert.Equal(90, valera.Mana);
        Assert.Equal(80, valera.Fatigue);
        Assert.Equal(50, valera.Money); // 200 - 150
    }

    [Fact]
    public void SingInMetro_WithBonus_WhenManaInRange()
    {
        var valera = new Valera(mana: 45, money : 100);

        valera.SingInMetro();

        Assert.Equal(1, valera.Cheerfulness);
        Assert.Equal(55, valera.Mana); // 45 + 10
        Assert.Equal(160, valera.Money); // 100 + 10 + 50 бонус
        Assert.Equal(20, valera.Fatigue);
    }

    [Fact]
    public void SingInMetro_WithoutBonus_WhenManaNotInRange()
    {
        var valera = new Valera(mana: 30, money : 100); 

        valera.SingInMetro();

        Assert.Equal(1, valera.Cheerfulness);
        Assert.Equal(40, valera.Mana); // 30 + 10
        Assert.Equal(110, valera.Money); // 100 + 10 (без бонуса)
        Assert.Equal(20, valera.Fatigue);
    }

    [Fact]
    public void Sleep_WithHealthBonus_WhenManaLow()
    {
        var valera = new Valera(health: 50, mana : 20, fatigue : 80);

        valera.Sleep();

        Assert.Equal(100, valera.Health); // 50 + 90 (но ограничено 100)
        Assert.Equal(0, valera.Mana); // 20 - 50 (но ограничено 0)
        Assert.Equal(10, valera.Fatigue); // 80 - 70
    }

    [Fact]
    public void Sleep_WithCheerfulnessPenalty_WhenManaHigh()
    {
        var valera = new Valera(mana: 80, cheerfulness : 5, fatigue : 60);

        valera.Sleep();

        Assert.Equal(2, valera.Cheerfulness); // 5 - 3
        Assert.Equal(30, valera.Mana); // 80 - 50
        Assert.Equal(0, valera.Fatigue); // 60 - 70 (но ограничено 0)
    }

    [Fact]
    public void PropertyClamping_WorksCorrectly()
    {
        var valera = new Valera();

        valera = new Valera(
            health: 150,
            mana : -10,
            cheerfulness : 15,
            fatigue : -5,
            money : -50
        );

        Assert.Equal(100, valera.Health);
        Assert.Equal(0, valera.Mana);
        Assert.Equal(10, valera.Cheerfulness);
        Assert.Equal(0, valera.Fatigue);
        Assert.Equal(0, valera.Money);
    }

    [Fact]
    public void IsAlive_ReturnsFalse_WhenHealthZero()
    {
        var valera = new Valera(health: 0);

        Assert.False(valera.IsAlive);
    }

    [Fact]
    public void IsAlive_ReturnsFalse_WhenFatigueMax()
    {
        var valera = new Valera(fatigue: 100);

        Assert.False(valera.IsAlive);
    }

    [Fact]
    public void IsAlive_ReturnsTrue_WhenConditionsMet()
    {
        var valera = new Valera(health: 50, fatigue : 50);

        Assert.True(valera.IsAlive);
    }
}
