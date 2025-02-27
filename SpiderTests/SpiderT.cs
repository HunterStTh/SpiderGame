using Xunit;
using SpiderGame;
using System;

namespace SpiderTests
{
    public class SpiderT
    {
        /*
        [Fact] – это атрибут, который помечает обычный тест. Тест с [Fact] не принимает параметров. Это просто метод, который запускается как тест.
        [Theory] – это атрибут, который помечает параметризованный тест. Он позволяет запускать тест с разными входными значениями.
        [InlineData(...)] – это атрибут, который передаёт конкретные параметры в [Theory]. Каждый [InlineData] создаёт отдельный запуск теста с указанными аргументами.
        */
        // Тест на корректную инициализацию
        [Fact]
        public void Spider_Initialization_SetsCorrectProperties()
        {
            var spider = new Spider("TestSpider", 100, 50, DateTime.Now);

            Assert.Equal("TestSpider", spider.Name);
            Assert.Equal(100, spider.Health);
            Assert.Equal(50, spider.Armor);
        }

        // Тест на атаку с разным оружием
        [Theory]
        [InlineData(WeaponType.Knife, 15)]
        [InlineData(WeaponType.Web, 10)]
        [InlineData(WeaponType.Net, 8)]
        public void Attack_WithDifferentWeapons_DealsCorrectDamage(WeaponType weapon, int expectedDamage)
        {
            var attacker = new Spider("Attacker", 100, 50, DateTime.Now);
            var target = new Spider("Target", 100, 0, DateTime.Now); // Броня 0, чтобы урон сразу шёл в здоровье
            attacker.SelectedWeapon = weapon;

            attacker.Attack(target);

            int expectedHealth = 100 - expectedDamage;
            Assert.Equal(expectedHealth, target.Health);
        }

        // Тест на атаку с учётом брони
        [Fact]
        public void Attack_WithArmor_ReducesArmorFirst()
        {
            var attacker = new Spider("A", 100, 50, DateTime.Now) { SelectedWeapon = WeaponType.Knife };
            var target = new Spider("B", 100, 15, DateTime.Now); // Броня 15

            attacker.Attack(target);

            Assert.Equal(0, target.Armor);
            Assert.Equal(100 - (15 - 15), target.Health); // Броня поглотила весь урон
        }
        // Тест на корректное отображение здоровья и брони
        [Fact]
        public void HealthInfo_ReturnsCorrectString()
        {
            var spider = new Spider("TestSpider", 100, 50, DateTime.Now);

            var healthInfo = spider.HealthInfo;

            Assert.Equal("HP: 100 | Armor: 50", healthInfo);
        }

        //Тест на обновление строки после атаки
        [Fact]
        public void HealthInfo_AfterAttack_ReturnsUpdatedString()
        {
            var attacker = new Spider("Attacker", 100, 50, DateTime.Now) { SelectedWeapon = WeaponType.Knife };
            var target = new Spider("Target", 100, 50, DateTime.Now);

            attacker.Attack(target); // Атакуем цель
            var healthInfo = target.HealthInfo;

            Assert.Equal("HP: 100 | Armor: 35", healthInfo); // Проверяем обновлённую строку
        }

        // Тест на обновление строки после атаки без брони
        [Fact]
        public void HealthInfo_AfterAttackWithArmor_ReturnsUpdatedString()
        {
            var attacker = new Spider("Attacker", 100, 50, DateTime.Now) { SelectedWeapon = WeaponType.Knife };
            var target = new Spider("Target", 100, 15, DateTime.Now); // Броня 15

            attacker.Attack(target); // Атакуем цель
            var healthInfo = target.HealthInfo;

            Assert.Equal("HP: 100 | Armor: 0", healthInfo); // Броня уменьшилась до 0, здоровье не изменилось
        }

        // Тест на обновление строки после смерти
        [Fact]
        public void HealthInfo_AfterDeath_ReturnsZeroHealth()
        {
            var attacker = new Spider("Attacker", 100, 50, DateTime.Now) { SelectedWeapon = WeaponType.Knife };
            var target = new Spider("Target", 10, 0, DateTime.Now); // Здоровье 10, брони нет

            attacker.Attack(target); // Атакуем цель (урон 15)
            var healthInfo = target.HealthInfo;

            Assert.Equal("HP: 0 | Armor: 0", healthInfo); // Здоровье уменьшилось до 0
        }
        // Тест на невозможность выбрать цель не из Targets
        [Fact]
        public void SelectedTarget_WhenNotInTargets_SetToNull()
        {
            var spider1 = new Spider("Spider1", 100, 50, DateTime.Now);
            var spider2 = new Spider("Spider2", 100, 50, DateTime.Now);

            spider1.SelectedTarget = spider2;

            Assert.Null(spider1.SelectedTarget);
        }
                [Fact]
        // Тест на невозможность выбрать паука, который умер или удален
        public void SelectedTarget_WhenTargetRemovedFromTargets_SetToNull()
        {
            var spider1 = new Spider("Spider1", 100, 50, DateTime.Now);
            var spider2 = new Spider("Spider2", 100, 50, DateTime.Now);
            spider1.Targets.Add(spider2);
            spider1.SelectedTarget = spider2;

            spider1.Targets.Remove(spider2);

            Assert.Null(spider1.SelectedTarget);
        }

    }
}
