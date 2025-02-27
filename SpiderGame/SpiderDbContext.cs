// Гаврилов Д
/*SpiderDbContext: Класс-наследник DbContext, отвечает за взаимодействие с базой данных.

DbSet<Spider>: Представляет таблицу Spiders в базе данных. Каждый объект Spider соответствует строке в таблице.

OnConfiguring: Настраивает подключение к SQLite (Data Source=spiders.db).

OnModelCreating: Определяет правила создания таблиц (например, автоинкремент для Id, значение по умолчанию для DateAdded).*/
using Microsoft.EntityFrameworkCore;

namespace SpiderGame
{
    // Класс, представляющий контекст базы данных Entity Framework Core.
    // Отвечает за взаимодействие с базой данных: создание, обновление, запросы.
    public class SpiderDbContext : DbContext
    {
        // DbSet для работы с таблицей Spiders. Каждый объект Spider соответствует записи в таблице.
        public DbSet<Spider> Spiders { get; set; }

        // Настройка подключения к базе данных SQLite.
        // "Data Source=spiders.db" — путь к файлу базы данных.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=spiders.db");

        // Настройка модели данных (схемы таблицы).
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ограничение длины поля Id до 8 символов 
            modelBuilder.Entity<Spider>()
                .Property(s => s.Id)
                .HasMaxLength(8);

            // Установка значения по умолчанию для поля DateAdded 
            modelBuilder.Entity<Spider>()
                .Property(s => s.DateAdded)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}