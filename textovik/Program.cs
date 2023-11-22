using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

// Модель "Figure"
[Serializable]
public class Figure
{
    public string Name { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public Figure(string name, double width, double height)
    {
        Name = name;
        Width = width;
        Height = height;
    }

    // Пустой конструктор нужен для десериализации XML
    public Figure() { }
}

// Класс для чтения и записи файла
public class FileManager
{
    private string filePath;

    public FileManager(string path)
    {
        filePath = path;
    }

    private string GetFileExtension()
    {
        return Path.GetExtension(filePath).ToLower();
    }

    public Figure LoadFile()
    {
        try
        {
            string extension = GetFileExtension();

            if (extension == ".txt")
            {
                // Чтение из текстового файла
                string[] lines = File.ReadAllLines(filePath);
                return new Figure(lines[0], Convert.ToDouble(lines[1]), Convert.ToDouble(lines[2]));
            }
            else if (extension == ".json")
            {
                // Чтение из JSON файла
                string jsonContent = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Figure>(jsonContent);
            }
            else if (extension == ".xml")
            {
                // Чтение из XML файла
                XmlSerializer serializer = new XmlSerializer(typeof(Figure));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    return (Figure)serializer.Deserialize(fileStream);
                }
            }

            Console.WriteLine("Неподдерживаемый формат файла");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
            return null;
        }
    }

    public void SaveFile(Figure figure)
    {
        try
        {
            string extension = GetFileExtension();

            if (extension == ".txt")
            {
                // Сохранение в текстовый файл
                string[] lines = { figure.Name, figure.Width.ToString(), figure.Height.ToString() };
                File.WriteAllLines(filePath, lines);
            }
            else if (extension == ".json")
            {
                // Сохранение в JSON файл
                string jsonContent = JsonSerializer.Serialize(figure);
                File.WriteAllText(filePath, jsonContent);
            }
            else if (extension == ".xml")
            {
                // Сохранение в XML файл
                XmlSerializer serializer = new XmlSerializer(typeof(Figure));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(fileStream, figure);
                }
            }

            Console.WriteLine("Файл успешно сохранен");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Введите путь к файлу:");
        string filePath = Console.ReadLine();

        FileManager fileManager = new FileManager(filePath);
        Figure figure = fileManager.LoadFile();

        if (figure != null)
        {
            Console.WriteLine($"Имя: {figure.Name}, Ширина: {figure.Width}, Высота: {figure.Height}");
        }

        Console.WriteLine("Нажмите F1 для сохранения файла, Escape для выхода.");
        ConsoleKeyInfo keyInfo;

        do
        {
            keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.F1)
            {
                fileManager.SaveFile(figure);
            }

        } while (keyInfo.Key != ConsoleKey.Escape);
    }
}

