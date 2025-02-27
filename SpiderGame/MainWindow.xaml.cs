// Гаврилов Д
/* Класс главного окна приложения.
Хранит коллекцию пауков (ObservableCollection<Spider>).
Создание новых пауков через InputWindow.
Обработка атак (проверка выбора цели и оружия).
Обновление списков целей для всех пауков (UpdateTargets).
Удаление мёртвых пауков и оповещение об этом.*/
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SpiderGame
{
    public partial class MainWindow : Window
    {
        // Коллекция пауков
        // ObservableCollection<Spider> - автоматически уведомляет UI об изменениях (добавление/удаление элементов)
        // Сама коллекция — ссылочный тип, а её элементы — ссылки на объекты Spider.
        private readonly SpiderDbContext _dbContext = new SpiderDbContext();
        public ObservableCollection<Spider> Spiders { get; } = new ObservableCollection<Spider>();

        // Конструктор главного окна
        public MainWindow()
        {
            using (var db = new SpiderDbContext())
            {
                db.Database.Migrate(); // Применяет миграции, если они не применены
            }
            InitializeComponent();
            DataContext = this; // Устанавливаем контекст данных
            LoadSpidersFromDb();
        }

        /*Обработчик кнопки "Добавить паука"        
        private void CreateSpiderButton_Click(object sender, RoutedEventArgs e)
        {
            // Передаем текущие имена при создании окна
            var existingNames = Spiders.Select(s => s.Name.ToLower()).ToList();
            // Передаем текущие имена при создании окна
            var inputWindow = new InputWindow(existingNames);
            // Если введено имя
            if (inputWindow.ShowDialog() == true) 
            {
                string newName = inputWindow.SpiderName;
                if (!existingNames.Contains(newName.ToLower()))
                {
                    // Создаем паука
                    var spider = new Spider(inputWindow.SpiderName, 100, 50, DateTime.Now); 
                    // Подписываемся на событие смерти
                    spider.SpiderDied += OnSpiderDied; 
                    // Добавляем паука в коллекцию
                    Spiders.Add(spider);
                    // Сохраняем в БД
                    using (var db = new SpiderDbContext())
                    {
                        db.Spiders.Add(spider);
                        db.SaveChanges();
                    }
                    // Добавляем в коллекцию UI
                    Spiders.Add(spider);
                    // Обновляем списки целей
                    UpdateTargets(); 
                }
                else
                {
                    MessageBox.Show("Паук с таким именем уже существует!");
                }
            }
        }*/

        // Обработчик кнопки "Атаковать"
        private void AttackButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Spider attacker) // Получаем паука из Tag кнопки
            {
                if (attacker.SelectedTarget == null) // Проверка выбора цели
                {
                    MessageBox.Show("Выберите цель!");
                    return;
                }
                // Проверка, что оружие выбрано
                if (attacker.SelectedWeapon == default(WeaponType)) // Проверка на значение по умолчанию
                {
                    MessageBox.Show("Выберите оружие для атаки!");
                    return;
                }
                attacker.Attack(attacker.SelectedTarget); // Выполняем атаку
                UpdateTargets(); // Обновляем списки целей
            }
        }
        // Обработчик кнопки открыть БД
        private void OpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            var dbWindow = new DatabaseWindow();
            dbWindow.ShowDialog(); // Блокирует выполнение до закрытия окна
            LoadSpidersFromDb(); // Обновляем данные после закрытия
        }
        private void LoadSpidersFromDb()
        {
            using (var db = new SpiderDbContext())
            {
                var spiders = db.Spiders.ToList();
                Spiders.Clear();
                foreach (var spider in spiders)
                {
                    spider.SpiderDied += OnSpiderDied; // Повторная подписка на событие смерти
                    Spiders.Add(spider);
                }
                UpdateTargets();
            }
        }

        // Обработчик события смерти паука
        private void OnSpiderDied(object sender, EventArgs e)
        {
            if (sender is Spider spider)
            {
                using (var db = new SpiderDbContext())
                {
                    var dbSpider = db.Spiders.FirstOrDefault(s => s.Id == spider.Id);
                    if (dbSpider != null)
                    {
                        db.Spiders.Remove(dbSpider); // Удаляем из БД
                        db.SaveChanges();
                    }
                }

                Spiders.Remove(spider); // Удаляем из коллекции UI
                UpdateTargets(); // Обновляем списки целей
                MessageBox.Show($"{spider.Name} погиб!"); // Сообщение
                spider.Dispose();
            }
        }


        // Обновление списков целей для всех пауков
        public void UpdateTargets()
        {
            foreach (var spider in Spiders)
            {
                var currentTarget = spider.SelectedTarget; // Сохраняем текущую цель

                // Фильтруем пауков, исключая текущего
                var newTargets = Spiders.Where(s => s != spider).ToList();

                // Удаляем старые цели, которых нет в новом списке
                foreach (var existingTarget in spider.Targets.ToList())
                {
                    if (!newTargets.Contains(existingTarget))
                    {
                        spider.Targets.Remove(existingTarget);
                    }
                }

                // Добавляем новые цели
                foreach (var target in newTargets)
                {
                    if (!spider.Targets.Contains(target))
                    {
                        spider.Targets.Add(target);
                    }
                }
                // Обновляем список целей
                spider.Targets = new ObservableCollection<Spider>(Spiders.Where(s => s != spider));
                // Восстанавливаем выбор цели, если она существует
                spider.SelectedTarget = spider.Targets.Contains(currentTarget) ? currentTarget : null;
            }
            using var db = new SpiderDbContext();
            db.SaveChanges();
        }
    }
}