// Гаврилов Д
// Класс, представляющий паука в игре
/* Реализует INotifyPropertyChanged для синхронизации с UI.
Свойства: Имя, Здоровье, Броня, Выбранное оружие, Цель.
Метод Attack() - расчёт урона с учётом брони.
Событие SpiderDied для оповещения о смерти.
Механизм автоматического обновления UI при изменении свойств.*/
// ссылочный типы
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SpiderGame
{
    // Класс Spider - ссылочный тип, он реализует интерфейс INotifyPropertyChanged для уведомления об изменениях свойств
    // При создании экземпляра  = new Spider.. выделяется память под обьект, 
    // а переменные хранят ссылку на него, а не сам объект.
    public class Spider : INotifyPropertyChanged //Когда свойство (например, Health) меняется, вызывается OnPropertyChanged(), и UI обновляется.
    {
        // Событие, которое вызывается при смерти паука
        public event EventHandler SpiderDied;

        // Статическая переменная для подсчёта количества пауков
        private static int _spiderCount = 0;

        // Приватные поля для хранения данных
        // string — ссылочный тип
        private string _name; // Имя паука
        // int - значимый тип
        private int _health; // Здоровье паука
        private int _armor; // Броня паука
        [NotMapped]
        private WeaponType _selectedWeapon; // Выбранное оружие
        [NotMapped]
        private Spider _selectedTarget; // Выбранная цель для атаки

        // Свойство Name (имя паука)
        // ссылочный тип
        public string Name
        {
            get => _name; // Возвращает текущее имя
            set
            {
                _name = value; // Устанавливает новое имя
                OnPropertyChanged(nameof(Name)); // Уведомляет об изменении свойства
            }
        }

        // Свойство Health (здоровье паука)
        // значимыq тип
        public int Health
        {
            get => _health; // Возвращает текущее здоровье
            private set // Устанавливается только внутри класса
            {
                _health = value; // Устанавливает новое здоровье
                OnPropertyChanged(nameof(Health)); // Уведомляет об изменении здоровья
                OnPropertyChanged(nameof(HealthInfo)); // Уведомляет об изменении строки HealthInfo
                if (_health <= 0) OnSpiderDied(); // Если здоровье <= 0, вызываем событие смерти
            }
        }

        // Свойство Armor (броня паука)
        public int Armor
        {
            get => _armor; // Возвращает текущую броню
            private set // Устанавливается только внутри класса
            {
                _armor = value; // Устанавливает новую броню
                OnPropertyChanged(nameof(Armor)); // Уведомляет об изменении брони
                OnPropertyChanged(nameof(HealthInfo)); // Уведомляет об изменении строки HealthInfo
            }
        }

        // Свойство HealthInfo (строка с информацией о здоровье и броне)
        public string HealthInfo => $"HP: {Health} | Armor: {Armor}";

        // Свойство SelectedWeapon (выбранное оружие)
        // SelectedTarget — свойство, хранящее ссылку на другой объект Spider.
        [NotMapped] // Это свойство не будет сохраняться в БД
        public WeaponType SelectedWeapon
        {
            get => _selectedWeapon; // Возвращает текущее оружие
            set
            {
                _selectedWeapon = value; // Устанавливает новое оружие
                OnPropertyChanged(); // Уведомляет об изменении (имя свойства определяется автоматически)
            }
        }

        // Свойство SelectedTarget (выбранная цель для атаки)
        [NotMapped] // Это свойство не будет сохраняться в БД
        public Spider SelectedTarget
        {
            get => _selectedTarget; // Возвращает текущую цель
            set
            {
                // Сохраняем цель только если она существует в списке Targets
                _selectedTarget = Targets.Contains(value) ? value : null;
                OnPropertyChanged(nameof(SelectedTarget)); // Уведомляет об изменении цели
            }
        }
        // ID паука
        public string Id { get; set; } = GenerateShortId();

        private static string GenerateShortId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        // Время создания паука
        public DateTime DateAdded { get; private set; }


        // Конструктор класса Spider
        public Spider(string name, int health, int armor, DateTime date)
        {
            // Генерация ID
            DateAdded = date; // Время создания паука
            Name = name; // Устанавливаем имя
            Health = health; // Устанавливаем здоровье
            Armor = armor; // Устанавливаем броню
            Targets = new ObservableCollection<Spider>(); // Инициализация коллекции целей
            Targets.CollectionChanged += OnTargetsCollectionChanged; // Подписываемся на событие
        }
        // Конструктор без параметров для EF Core
        public Spider()
        {
            /*            
            Targets = new ObservableCollection<Spider>();
            Targets.CollectionChanged += OnTargetsCollectionChanged;
            */

        }

        // Приватное поле для хранения списка целей
        private ObservableCollection<Spider> _targets = new ObservableCollection<Spider>();

        // Свойство Targets (список целей для атаки)
        [NotMapped] // Исключаем из БД
        public ObservableCollection<Spider> Targets
        {
            get => _targets; // Возвращает текущий список целей
            set
            {
                _targets = value; // Устанавливает новый список целей
                OnPropertyChanged(); // Уведомляет об изменении (имя свойства определяется автоматически)
            }
        }

        // Метод Attack (атака выбранной цели)
        // принимает target по ссылке. 
        public void Attack(Spider target)
        {
            // если паук не создан или умер
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            // Определяем урон в зависимости от выбранного оружия
            int damage = SelectedWeapon switch
            {
                WeaponType.Knife => 15, // Урон от ножа
                WeaponType.Web => 10,   // Урон от паутины
                WeaponType.Net => 8,    // Урон от сети
                _ => 0
            };


            using (var db = new SpiderDbContext())
            {
                // Загружаем цель из базы данных с отслеживанием
                var dbTarget = db.Spiders.FirstOrDefault(s => s.Id == target.Id);
                if (dbTarget == null) return;

                // Обновляем данные только в dbTarget 
                if (dbTarget.Armor > 0)
                {
                    dbTarget.Armor -= damage;
                    if (dbTarget.Armor < 0)
                    {
                        dbTarget.Health += dbTarget.Armor;
                        dbTarget.Armor = 0;
                    }
                }
                else
                {
                    dbTarget.Health -= damage;
                }

                if (dbTarget.Health < 0)
                    dbTarget.Health = 0;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Обрабатываем случай, если паук был удален
                    foreach (var entry in ex.Entries)
                    {
                        entry.Reload(); // Перезагружаем данные
                        if (entry.Entity is Spider spider && spider.Health <= 0)
                        {
                            // Удаляем паука, если он мертв
                            db.Spiders.Remove(spider);
                            db.SaveChanges();
                        }
                    }
                }

                // Синхронизируем локальный объект с данными из БД
                target.Armor = dbTarget.Armor;
                target.Health = dbTarget.Health;
                target.OnPropertyChanged(nameof(HealthInfo));
            }
        }
        // Реализация IDisposable для уменьшения счётчика при удалении
        public void Dispose()
        {
            Interlocked.Decrement(ref _spiderCount);
        }

        // Метод OnSpiderDied (вызывается при смерти паука)
        // Проверяем подписчиков на событие знаком '?'
        // Может быть унаследован в производных классах, так как virtual
        // Он не должен быть доступен извне, но может быть переопределён в наследниках.
        // Invoke вызывает все подписанные методы в порядке подписки.
        protected virtual void OnSpiderDied() => SpiderDied?.Invoke(this, EventArgs.Empty);

        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Метод OnPropertyChanged (уведомляет об изменении свойства)
        // [CallerMemberName] Автоматически подставляет имя свойства, вызвавшего метод.
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void OnTargetsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (Spider removedSpider in e.OldItems)
                {
                    if (SelectedTarget == removedSpider)
                    {
                        SelectedTarget = null; // Сбрасываем SelectedTarget, если цель удалена
                    }
                }
            }
        }
    }

    // Перечисление WeaponType (типы оружия)
    public enum WeaponType { None, Knife, Web, Net }
}
//toString
//