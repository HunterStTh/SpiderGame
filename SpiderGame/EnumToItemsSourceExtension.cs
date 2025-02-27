// Гаврилов Д
/*EnumToItemsSourceExtension.cs
Используется для работы с перечислениями в XAML.
Преобразует значения enum в список строк (исключая значение "None").
Используется для привязки элементов управления (например, ComboBox) к enum.
::Отображение типов оружия (Knife, Web, Net) в выпадающем списке.*/
using System.Windows.Markup; // Подключение пространства имен для работы с MarkupExtension

namespace SpiderGame
{
    // Класс EnumToItemsSource, который наследуется от MarkupExtension
    public class EnumToItemsSource : MarkupExtension
    {
        // Приватное поле для хранения типа перечисления
        private readonly Type _type;

        // Конструктор, принимающий тип перечисления
        // Type type - параметр конструктора, указывает на то, что конструктор принимает объект типа Type
        public EnumToItemsSource(Type type) => _type = type; // присваивание значения параметра полю класса

        // Переопределение метода ProvideValue, который возвращает значения перечисления
        public override object ProvideValue(IServiceProvider serviceProvider) =>
            Enum.GetValues(_type) // Получаем все значения перечисления
                .Cast<object>()   // Приводим их к типу object
                .Where(e => e.ToString() != "None") // Исключаем "None"
                .Select(e => e.ToString()) // Преобразуем каждое значение в строку
                .ToList(); // Возвращаем список строк
    }
}