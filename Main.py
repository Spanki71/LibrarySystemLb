# Информационная система "Библиотека"
# Автор: [Ваше имя]
# Группа: [Ваша группа]

class Book:
    """Класс, представляющий книгу в библиотеке"""
    def __init__(self, book_id, title, author, year, genre, is_available=True):
        self.book_id = book_id
        self.title = title
        self.author = author
        self.year = year
        self.genre = genre
        self.is_available = is_available

    def __str__(self):
        status = "Доступна" if self.is_available else "Выдана"
        return f"{self.book_id}. {self.title} - {self.author} ({self.year}) [{status}]"


class Reader:
    """Класс, представляющий читателя библиотеки"""
    def __init__(self, reader_id, full_name, card_number, phone):
        self.reader_id = reader_id
        self.full_name = full_name
        self.card_number = card_number
        self.phone = phone

    def __str__(self):
        return f"{self.reader_id}. {self.full_name} (Билет: {self.card_number})"


class Loan:
    """Класс, представляющий выдачу книги читателю"""
    def __init__(self, loan_id, reader_id, book_id, loan_date, return_date=None):
        self.loan_id = loan_id
        self.reader_id = reader_id
        self.book_id = book_id
        self.loan_date = loan_date
        self.return_date = return_date


class Library:
    """Основной класс библиотечной системы"""
    def __init__(self):
        self.books = []
        self.readers = []
        self.loans = []
        self.next_book_id = 1
        self.next_reader_id = 1
        self.next_loan_id = 1

    # ---------- Управление книгами ----------
    def add_book(self, title, author, year, genre):
        """Добавление новой книги в библиотеку"""
        book = Book(self.next_book_id, title, author, year, genre)
        self.books.append(book)
        self.next_book_id += 1
        print(f"✓ Книга '{title}' добавлена в библиотеку (ID: {book.book_id})")
        return book

    def list_books(self, show_all=True):
        """Вывод списка всех книг"""
        if not self.books:
            print("В библиотеке пока нет книг.")
            return
        print("\n" + "="*60)
        print("КАТАЛОГ КНИГ")
        print("="*60)
        for book in self.books:
            if show_all or book.is_available:
                print(book)
        print("="*60)

    def find_book_by_id(self, book_id):
        """Поиск книги по ID"""
        for book in self.books:
            if book.book_id == book_id:
                return book
        return None

    def find_book_by_title(self, title):
        """Поиск книги по названию"""
        results = [book for book in self.books if title.lower() in book.title.lower()]
        return results

    # ---------- Управление читателями ----------
    def add_reader(self, full_name, card_number, phone):
        """Добавление нового читателя"""
        reader = Reader(self.next_reader_id, full_name, card_number, phone)
        self.readers.append(reader)
        self.next_reader_id += 1
        print(f"✓ Читатель '{full_name}' добавлен (Билет: {card_number})")
        return reader

    def list_readers(self):
        """Вывод списка всех читателей"""
        if not self.readers:
            print("Читателей пока нет.")
            return
        print("\n" + "="*60)
        print("СПИСОК ЧИТАТЕЛЕЙ")
        print("="*60)
        for reader in self.readers:
            print(reader)
        print("="*60)

    def find_reader_by_id(self, reader_id):
        """Поиск читателя по ID"""
        for reader in self.readers:
            if reader.reader_id == reader_id:
                return reader
        return None

    # ---------- Операции выдачи и возврата ----------
    def issue_book(self, reader_id, book_id, loan_date):
        """Выдача книги читателю"""
        reader = self.find_reader_by_id(reader_id)
        book = self.find_book_by_id(book_id)

        if not reader:
            print(f"✗ Ошибка: Читатель с ID {reader_id} не найден.")
            return None
        if not book:
            print(f"✗ Ошибка: Книга с ID {book_id} не найдена.")
            return None
        if not book.is_available:
            print(f"✗ Ошибка: Книга '{book.title}' уже выдана другому читателю.")
            return None

        # Проверяем, нет ли у читателя долгов (невозвращенных книг)
        active_loans = [loan for loan in self.loans if loan.reader_id == reader_id and loan.return_date is None]
        if len(active_loans) >= 5:
            print(f"✗ Ошибка: Читатель '{reader.full_name}' уже имеет {len(active_loans)} книг. Максимум 5.")
            return None

        # Выдаем книгу
        loan = Loan(self.next_loan_id, reader_id, book_id, loan_date)
        self.loans.append(loan)
        book.is_available = False
        self.next_loan_id += 1

        print(f"✓ Книга '{book.title}' выдана читателю '{reader.full_name}'")
        return loan

    def return_book(self, book_id, return_date):
        """Возврат книги в библиотеку"""
        book = self.find_book_by_id(book_id)
        if not book:
            print(f"✗ Ошибка: Книга с ID {book_id} не найдена.")
            return False

        # Ищем активную выдачу
        active_loan = None
        for loan in self.loans:
            if loan.book_id == book_id and loan.return_date is None:
                active_loan = loan
                break

        if not active_loan:
            print(f"✗ Ошибка: Книга '{book.title}' не была выдана.")
            return False

        active_loan.return_date = return_date
        book.is_available = True
        print(f"✓ Книга '{book.title}' возвращена в библиотеку")
        return True

    def list_active_loans(self):
        """Вывод списка активных выдач"""
        active_loans = [loan for loan in self.loans if loan.return_date is None]
        if not active_loans:
            print("Нет активных выдач.")
            return

        print("\n" + "="*60)
        print("АКТИВНЫЕ ВЫДАЧИ")
        print("="*60)
        for loan in active_loans:
            reader = self.find_reader_by_id(loan.reader_id)
            book = self.find_book_by_id(loan.book_id)
            reader_name = reader.full_name if reader else "Неизвестен"
            book_title = book.title if book else "Неизвестна"
            print(f"  Читатель: {reader_name} | Книга: {book_title} | Дата выдачи: {loan.loan_date}")
        print("="*60)

    def get_reader_history(self, reader_id):
        """История выдач конкретного читателя"""
        reader = self.find_reader_by_id(reader_id)
        if not reader:
            print(f"Читатель с ID {reader_id} не найден.")
            return

        reader_loans = [loan for loan in self.loans if loan.reader_id == reader_id]
        if not reader_loans:
            print(f"У читателя '{reader.full_name}' нет истории выдач.")
            return

        print(f"\n=== ИСТОРИЯ ВЫДАЧ: {reader.full_name} ===")
        for loan in reader_loans:
            book = self.find_book_by_id(loan.book_id)
            book_title = book.title if book else "Неизвестна"
            status = "Возвращена" if loan.return_date else "На руках"
            return_info = f" (возврат: {loan.return_date})" if loan.return_date else ""
            print(f"  {loan.loan_date}: '{book_title}' - {status}{return_info}")


# ---------- Функция для демонстрации работы системы ----------
def main():
    """Главная функция для демонстрации работы ИС "Библиотека" """
    print("="*60)
    print("ИНФОРМАЦИОННАЯ СИСТЕМА «БИБЛИОТЕКА»")
    print("Версия 1.0")
    print("="*60)

    library = Library()

    # 1. Добавляем книги
    print("\n>>> ДОБАВЛЕНИЕ КНИГ")
    library.add_book("Война и мир", "Лев Толстой", 1869, "Роман")
    library.add_book("Преступление и наказание", "Фёдор Достоевский", 1866, "Роман")
    library.add_book("Мастер и Маргарита", "Михаил Булгаков", 1967, "Роман")
    library.add_book("1984", "Джордж Оруэлл", 1949, "Антиутопия")
    library.add_book("Гарри Поттер и философский камень", "Дж.К. Роулинг", 1997, "Фэнтези")

    # 2. Добавляем читателей
    print("\n>>> ДОБАВЛЕНИЕ ЧИТАТЕЛЕЙ")
    library.add_reader("Иванов Иван Иванович", "LIB-001", "+7 (999) 123-45-67")
    library.add_reader("Петрова Анна Сергеевна", "LIB-002", "+7 (999) 234-56-78")
    library.add_reader("Сидоров Петр Алексеевич", "LIB-003", "+7 (999) 345-67-89")

    # 3. Выводим список книг
    library.list_books()

    # 4. Выводим список читателей
    library.list_readers()

    # 5. Выдаем книги
    print("\n>>> ВЫДАЧА КНИГ")
    library.issue_book(1, 1, "2024-10-01")  # Иванов берет "Войну и мир"
    library.issue_book(1, 3, "2024-10-01")  # Иванов берет "Мастер и Маргариту"
    library.issue_book(2, 2, "2024-10-02")  # Петрова берет "Преступление и наказание"

    # 6. Список активных выдач
    library.list_active_loans()

    # 7. Возвращаем книгу
    print("\n>>> ВОЗВРАТ КНИГ")
    library.return_book(1, "2024-10-15")   # Возврат "Войны и мира"

    # 8. Снова список активных выдач
    library.list_active_loans()

    # 9. История выдач читателя
    library.get_reader_history(1)

    # 10. Итоговый каталог книг
    library.list_books()


if __name__ == "__main__":
    main()