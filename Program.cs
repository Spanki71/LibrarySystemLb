using System;
using System.Collections.Generic;
using System.Linq;

namespace LibrarySystem
{
    /// <summary>
    /// Класс, представляющий книгу в библиотеке
    /// </summary>
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; }

        public Book(int bookId, string title, string author, int year, string genre, bool isAvailable = true)
        {
            BookId = bookId;
            Title = title;
            Author = author;
            Year = year;
            Genre = genre;
            IsAvailable = isAvailable;
        }

        public override string ToString()
        {
            string status = IsAvailable ? "Доступна" : "Выдана";
            return $"{BookId}. {Title} - {Author} ({Year}) [{status}]";
        }
    }

    /// <summary>
    /// Класс, представляющий читателя библиотеки
    /// </summary>
    public class Reader
    {
        public int ReaderId { get; set; }
        public string FullName { get; set; }
        public string CardNumber { get; set; }
        public string Phone { get; set; }

        public Reader(int readerId, string fullName, string cardNumber, string phone)
        {
            ReaderId = readerId;
            FullName = fullName;
            CardNumber = cardNumber;
            Phone = phone;
        }

        public override string ToString()
        {
            return $"{ReaderId}. {FullName} (Билет: {CardNumber})";
        }
    }

    /// <summary>
    /// Класс, представляющий выдачу книги читателю
    /// </summary>
    public class Loan
    {
        public int LoanId { get; set; }
        public int ReaderId { get; set; }
        public int BookId { get; set; }
        public string LoanDate { get; set; }
        public string ReturnDate { get; set; }

        public Loan(int loanId, int readerId, int bookId, string loanDate, string returnDate = null)
        {
            LoanId = loanId;
            ReaderId = readerId;
            BookId = bookId;
            LoanDate = loanDate;
            ReturnDate = returnDate;
        }
    }

    /// <summary>
    /// Основной класс библиотечной системы
    /// </summary>
    public class Library
    {
        private List<Book> books = new List<Book>();
        private List<Reader> readers = new List<Reader>();
        private List<Loan> loans = new List<Loan>();
        private int nextBookId = 1;
        private int nextReaderId = 1;
        private int nextLoanId = 1;

        // ---------- Управление книгами ----------
        public void AddBook(string title, string author, int year, string genre)
        {
            var book = new Book(nextBookId, title, author, year, genre);
            books.Add(book);
            nextBookId++;
            Console.WriteLine($"✓ Книга '{title}' добавлена в библиотеку (ID: {book.BookId})");
        }

        public void ListBooks(bool showAll = true)
        {
            if (!books.Any())
            {
                Console.WriteLine("В библиотеке пока нет книг.");
                return;
            }

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("КАТАЛОГ КНИГ");
            Console.WriteLine(new string('=', 60));

            foreach (var book in books)
            {
                if (showAll || book.IsAvailable)
                {
                    Console.WriteLine(book);
                }
            }
            Console.WriteLine(new string('=', 60));
        }

        public Book FindBookById(int bookId)
        {
            return books.FirstOrDefault(b => b.BookId == bookId);
        }

        public List<Book> FindBookByTitle(string title)
        {
            return books.Where(b => b.Title.ToLower().Contains(title.ToLower())).ToList();
        }

        // ---------- Управление читателями ----------
        public void AddReader(string fullName, string cardNumber, string phone)
        {
            var reader = new Reader(nextReaderId, fullName, cardNumber, phone);
            readers.Add(reader);
            nextReaderId++;
            Console.WriteLine($"✓ Читатель '{fullName}' добавлен (Билет: {cardNumber})");
        }

        public void ListReaders()
        {
            if (!readers.Any())
            {
                Console.WriteLine("Читателей пока нет.");
                return;
            }

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("СПИСОК ЧИТАТЕЛЕЙ");
            Console.WriteLine(new string('=', 60));

            foreach (var reader in readers)
            {
                Console.WriteLine(reader);
            }
            Console.WriteLine(new string('=', 60));
        }

        public Reader FindReaderById(int readerId)
        {
            return readers.FirstOrDefault(r => r.ReaderId == readerId);
        }

        // ---------- Операции выдачи и возврата ----------
        public void IssueBook(int readerId, int bookId, string loanDate)
        {
            var reader = FindReaderById(readerId);
            var book = FindBookById(bookId);

            if (reader == null)
            {
                Console.WriteLine($"✗ Ошибка: Читатель с ID {readerId} не найден.");
                return;
            }

            if (book == null)
            {
                Console.WriteLine($"✗ Ошибка: Книга с ID {bookId} не найдена.");
                return;
            }

            if (!book.IsAvailable)
            {
                Console.WriteLine($"✗ Ошибка: Книга '{book.Title}' уже выдана другому читателю.");
                return;
            }

            // Проверяем, нет ли у читателя долгов (невозвращенных книг)
            var activeLoans = loans.Where(l => l.ReaderId == readerId && l.ReturnDate == null).ToList();
            if (activeLoans.Count >= 5)
            {
                Console.WriteLine($"✗ Ошибка: Читатель '{reader.FullName}' уже имеет {activeLoans.Count} книг. Максимум 5.");
                return;
            }

            // Выдаем книгу
            var loan = new Loan(nextLoanId, readerId, bookId, loanDate);
            loans.Add(loan);
            book.IsAvailable = false;
            nextLoanId++;

            Console.WriteLine($"✓ Книга '{book.Title}' выдана читателю '{reader.FullName}'");
        }

        public void ReturnBook(int bookId, string returnDate)
        {
            var book = FindBookById(bookId);
            if (book == null)
            {
                Console.WriteLine($"✗ Ошибка: Книга с ID {bookId} не найдена.");
                return;
            }

            // Ищем активную выдачу
            var activeLoan = loans.FirstOrDefault(l => l.BookId == bookId && l.ReturnDate == null);

            if (activeLoan == null)
            {
                Console.WriteLine($"✗ Ошибка: Книга '{book.Title}' не была выдана.");
                return;
            }

            activeLoan.ReturnDate = returnDate;
            book.IsAvailable = true;
            Console.WriteLine($"✓ Книга '{book.Title}' возвращена в библиотеку");
        }

        public void ListActiveLoans()
        {
            var activeLoans = loans.Where(l => l.ReturnDate == null).ToList();

            if (!activeLoans.Any())
            {
                Console.WriteLine("Нет активных выдач.");
                return;
            }

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("АКТИВНЫЕ ВЫДАЧИ");
            Console.WriteLine(new string('=', 60));

            foreach (var loan in activeLoans)
            {
                var reader = FindReaderById(loan.ReaderId);
                var book = FindBookById(loan.BookId);
                string readerName = reader?.FullName ?? "Неизвестен";
                string bookTitle = book?.Title ?? "Неизвестна";
                Console.WriteLine($"  Читатель: {readerName} | Книга: {bookTitle} | Дата выдачи: {loan.LoanDate}");
            }
            Console.WriteLine(new string('=', 60));
        }

        public void GetReaderHistory(int readerId)
        {
            var reader = FindReaderById(readerId);
            if (reader == null)
            {
                Console.WriteLine($"Читатель с ID {readerId} не найден.");
                return;
            }

            var readerLoans = loans.Where(l => l.ReaderId == readerId).ToList();

            if (!readerLoans.Any())
            {
                Console.WriteLine($"У читателя '{reader.FullName}' нет истории выдач.");
                return;
            }

            Console.WriteLine($"\n=== ИСТОРИЯ ВЫДАЧ: {reader.FullName} ===");
            foreach (var loan in readerLoans)
            {
                var book = FindBookById(loan.BookId);
                string bookTitle = book?.Title ?? "Неизвестна";
                string status = loan.ReturnDate != null ? "Возвращена" : "На руках";
                string returnInfo = loan.ReturnDate != null ? $" (возврат: {loan.ReturnDate})" : "";
                Console.WriteLine($"  {loan.LoanDate}: '{bookTitle}' - {status}{returnInfo}");
            }
        }
    }

    /// <summary>
    /// Главный класс программы
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("ИНФОРМАЦИОННАЯ СИСТЕМА «БИБЛИОТЕКА»");
            Console.WriteLine("Версия 1.0");
            Console.WriteLine(new string('=', 60));

            var library = new Library();

            // 1. Добавляем книги
            Console.WriteLine("\n>>> ДОБАВЛЕНИЕ КНИГ");
            library.AddBook("Война и мир", "Лев Толстой", 1869, "Роман");
            library.AddBook("Преступление и наказание", "Фёдор Достоевский", 1866, "Роман");
            library.AddBook("Мастер и Маргарита", "Михаил Булгаков", 1967, "Роман");
            library.AddBook("1984", "Джордж Оруэлл", 1949, "Антиутопия");
            library.AddBook("Гарри Поттер и философский камень", "Дж.К. Роулинг", 1997, "Фэнтези");

            // 2. Добавляем читателей
            Console.WriteLine("\n>>> ДОБАВЛЕНИЕ ЧИТАТЕЛЕЙ");
            library.AddReader("Иванов Иван Иванович", "LIB-001", "+7 (999) 123-45-67");
            library.AddReader("Петрова Анна Сергеевна", "LIB-002", "+7 (999) 234-56-78");
            library.AddReader("Сидоров Петр Алексеевич", "LIB-003", "+7 (999) 345-67-89");

            // 3. Выводим список книг
            library.ListBooks();

            // 4. Выводим список читателей
            library.ListReaders();

            // 5. Выдаем книги
            Console.WriteLine("\n>>> ВЫДАЧА КНИГ");
            library.IssueBook(1, 1, "2024-10-01");  // Иванов берет "Войну и мир"
            library.IssueBook(1, 3, "2024-10-01");  // Иванов берет "Мастер и Маргариту"
            library.IssueBook(2, 2, "2024-10-02");  // Петрова берет "Преступление и наказание"

            // 6. Список активных выдач
            library.ListActiveLoans();

            // 7. Возвращаем книгу
            Console.WriteLine("\n>>> ВОЗВРАТ КНИГ");
            library.ReturnBook(1, "2024-10-15");    // Возврат "Войны и мира"

            // 8. Снова список активных выдач
            library.ListActiveLoans();

            // 9. История выдач читателя
            library.GetReaderHistory(1);

            // 10. Итоговый каталог книг
            library.ListBooks();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}