namespace valera3.Models
{
    public class Valera
    {
        public int Id { get; set; }
        private int _health;
        private int _mana;
        private int _cheerfulness;
        private int _fatigue;
        private int _money;

        // Связь с пользователем
        public int UserId { get; set; }
        public User? User { get; set; }

        public int Health
        {
            get => _health;
            set => _health = Math.Clamp(value, 0, 100);
        }

        public int Mana
        {
            get => _mana;
            set => _mana = Math.Clamp(value, 0, 100);
        }

        public int Cheerfulness
        {
            get => _cheerfulness;
            set => _cheerfulness = Math.Clamp(value, -10, 10);
        }

        public int Fatigue
        {
            get => _fatigue;
            set => _fatigue = Math.Clamp(value, 0, 100);
        }

        public int Money
        {
            get => _money;
            set => _money = value < 0 ? 0 : value;
        }

        public Valera(int health = 100, int mana = 0, int cheerfulness = 0, int fatigue = 0, int money = 100)
        {
            Health = health;
            Mana = mana;
            Cheerfulness = cheerfulness;
            Fatigue = fatigue;
            Money = money;
        }

        /// <summary>
        /// Пойти на работу (Можно только если алкоголь < 50 а усталость < 10)
        /// </summary>
        public bool GoToWork()
        {
            if (Mana >= 50 || Fatigue >= 10)
                return false;

            Cheerfulness -= 5;
            Mana -= 30;
            Money += 100;
            Fatigue += 70;
            return true;
        }

        /// <summary>
        /// Созерцать природу
        /// </summary>
        public bool ContemplateNature()
        {
            Cheerfulness += 1;
            Mana -= 10;
            Fatigue += 10;
            return true;
        }

        /// <summary>
        /// Пить вино и смотреть сериал
        /// </summary>
        public bool DrinkWineAndWatchSeries()
        {
            if (Money < 20)
                return false;

            Cheerfulness -= 1;
            Mana += 30;
            Fatigue += 10;
            Health -= 5;
            Money -= 20;
            return true;
        }

        /// <summary>
        /// Сходить в бар
        /// </summary>
        public bool GoToBar()
        {
            if (Money < 100)
                return false;

            Cheerfulness += 1;
            Mana += 60;
            Fatigue += 40;
            Health -= 10;
            Money -= 100;
            return true;
        }

        /// <summary>
        /// Выпить с маргинальными личностями
        /// </summary>
        public bool DrinkWithMarginals()
        {
            if (Money < 150)
                return false;

            Cheerfulness += 5;
            Health -= 80;
            Mana += 90;
            Fatigue += 80;
            Money -= 150;
            return true;
        }

        /// <summary>
        /// Петь в метро
        /// </summary>
        public bool SingInMetro()
        {
            Cheerfulness += 1;
            Mana += 10;

            Money += 10;

            if (Mana > 40 && Mana < 70)
            {
                Money += 50;
            }

            Fatigue += 20;
            return true;
        }

        /// <summary>
        /// Спать
        /// </summary>
        public bool Sleep()
        {
            if (Mana < 30)
            {
                Health += 90;
            }

            if (Mana > 70)
            {
                Cheerfulness -= 3;
            }

            Mana -= 50;
            Fatigue -= 70;
            return true;
        }

        /// <summary>
        /// Проверка, жив ли Валерa
        /// </summary>
        public bool IsAlive => Health > 0 && Fatigue < 100;

        /// <summary>
        /// Получить текущее состояние Валеры
        /// </summary>
        public override string ToString()
        {
            return $"Здоровье: {Health}, Мана: {Mana}, Жизнерадостность: {Cheerfulness}, " +
                   $"Усталость: {Fatigue}, Деньги: ${Money}, Жив: {IsAlive}";
        }
    }
}
