# Week 1 · Day 6 — Weekly Exam: ADO.NET
# Exam · Экзамен · Имтиҳон

**Languages / Языки / Забонҳо:** [English](#en) · [Русский](#ru) · [Тоҷикӣ](#tj)

---

<a id="en"></a>
## 🇬🇧 English

**Format:** practical, individual · **Total:** 100 points
**Stack:** .NET console app · `Npgsql` · `System.Data` · PostgreSQL
**Allowed:** official documentation and your own BookNest project.
**Not allowed:** copying from another student, AI assistants, Dapper, EF Core.

The exam uses a **new domain** — not BookNest. You prove you can build an ADO.NET application from
scratch, from the connection to the final report.

---

### Scenario — MiniShop

Build a console application that manages a small shop's categories, products and stock movements.
Create a database named `minishop`.

**Required schema** (create it from code, safe to re-run):
```sql
categories (
    id    SERIAL PRIMARY KEY,
    name  VARCHAR(100) NOT NULL UNIQUE
)

products (
    id           SERIAL PRIMARY KEY,
    name         VARCHAR(150) NOT NULL,
    price        NUMERIC(10,2) NOT NULL CHECK (price > 0),
    quantity     INT NOT NULL DEFAULT 0 CHECK (quantity >= 0),
    category_id  INT NOT NULL REFERENCES categories(id)
)

stock_movements (
    id          SERIAL PRIMARY KEY,
    product_id  INT NOT NULL REFERENCES products(id),
    change_qty  INT NOT NULL CHECK (change_qty <> 0),   -- positive = restock, negative = sale
    moved_at    TIMESTAMP NOT NULL DEFAULT NOW()
)
```

---

### Task 1 — Structure, connection and seed (10 pts)

- Organize the project into folders: `Models/`, `Data/`, `Repositories/`, `Services/`, `UI/`.
- The connection string exists in **one** place, in a `Database` helper that returns a connection.
- On startup, test the connection (exit gracefully with a message if it fails), create the schema,
  and seed **3 categories** and **6 products**.
- Starting the app a second time must **not** create duplicates.
- `Program.cs` contains no SQL.

---

### Task 2 — Product and category CRUD (20 pts)

**`ProductRepository`:**

| Method | Returns | Description |
|--------|---------|-------------|
| `GetAll()` | `List<Product>` | All products |
| `GetById(int id)` | `Product?` | One product or `null` |
| `Add(Product p)` | `int` | Inserts and returns the new id |
| `Update(Product p)` | `bool` | `true` if a row was changed |
| `Delete(int id)` | `bool` | `true` if a row was deleted |

**`CategoryRepository`:** the same five methods for categories.

**Requirements:**
- Every command is parameterized.
- Validate before saving: name required, price greater than 0, quantity 0 or more. Print all errors.
- Adding a category with an existing name prints `Category already exists.`
- Deleting a category that still has products prints `Cannot delete: category has products.`
- Both messages come from catching `PostgresException` and checking its `SqlState`.
- A menu lets the user list, find, add, update and delete products and categories.

---

### Task 3 — Search and filters (10 pts)

Add to `ProductRepository`:

| Method | Description |
|--------|-------------|
| `SearchByName(string term)` | Case-insensitive, matches part of the name |
| `Filter(int? categoryId, decimal? minPrice, decimal? maxPrice)` | The `WHERE` clause contains **only** the filters that were provided |

**Requirements:**
- Parameterized. Searching for `'; DROP TABLE products; --` must return nothing and change nothing.
- Searching for and adding a product named `O'Brien's Cable` works correctly.

---

### Task 4 — Stock operations with a transaction (20 pts)

Create `StockService` with two methods.

**`RegisterSale(int productId, int qty)`**

| Situation | Message |
|-----------|---------|
| `qty` is 0 or negative | `Quantity must be greater than 0.` |
| Product not found | `Product not found.` |
| `qty` is more than the stock | `Cannot sell: only {n} in stock.` |
| Success | `Sale registered. Remaining: {n}` |

**`Restock(int productId, int qty)`**

| Situation | Message |
|-----------|---------|
| `qty` is 0 or negative | `Quantity must be greater than 0.` |
| Product not found | `Product not found.` |
| Success | `Restocked. New quantity: {n}` |

**Requirements:**
- For each operation, the movement insert and the quantity update happen in **one transaction**
  on **one connection**. If anything fails, nothing is saved.
- A rejected sale changes nothing in the database.
- Add menu items for both operations.

---

### Task 5 — Reports (20 pts)

Each report is **one SQL query**, using joins and grouping in SQL, not in C#.

| Report | Description |
|--------|-------------|
| Products with category | Name, price, quantity and category name for every product |
| Low stock | Products with `quantity` below a threshold entered by the user (parameterized) |
| Category summary | For every category: number of products and total stock value (`quantity × price`). Categories with **no products** must appear with `0` |
| Sales per product | Total units sold per product, highest first. Products with **no sales** must appear with `0` |

**Expected output (example):**
```
Category summary:
Accessories — 3 product(s) — stock value: 412.50
Cables      — 2 product(s) — stock value: 96.00
Spare parts — 0 product(s) — stock value: 0.00
```

---

### Task 6 — DataSet feature (10 pts)

1. Load all products into a `DataSet` with `NpgsqlDataAdapter`. Use a single-table select command and
   a `NpgsqlCommandBuilder`.
2. Ask the user for a category. In memory, raise the price of **every product in that category** by 10%
   (rounded to 2 decimals).
3. Print how many rows are pending as Modified.
4. Save with a **single** `adapter.Update`.
5. Prove it worked: load the products into a **new** `DataSet` and print the new prices.

---

### Task 7 — Robustness and README (10 pts)

- No crash on non-numeric input, unknown ids, empty input or a lost connection during a command.
- A `README.md` explains: how to create the database, how to run, the list of menu features, and the
  schema.
- The project is cleaned up: no dead or commented-out code, no `bin/` or `obj/` in the repository.

---

### Self-check — what the examiner will run

| # | Action | Expected |
|---|--------|----------|
| 1 | Start the app on an empty database | Schema created, seed inserted, no errors |
| 2 | Start it again | No duplicate categories or products |
| 3 | Add a product named `O'Brien's Cable` | Saved |
| 4 | Search for `'; DROP TABLE products; --` | Nothing found; the `products` table still exists |
| 5 | Sell more units than are in stock | Rejected; quantity and movements unchanged |
| 6 | Sell a valid quantity | Success; quantity decreased; exactly one new movement |
| 7 | Delete a category that has products | `Cannot delete: category has products.` |
| 8 | Open all 4 reports | Correct data; empty category and unsold products appear with `0` |
| 9 | DataSet: raise prices by 10% for one category | Pending count shown; one `Update`; new prices after reload |

---

### Grading (100 pts)

| Task | Pts |
|------|-----|
| 1 — Structure, connection, seed | 10 |
| 2 — CRUD | 20 |
| 3 — Search and filters | 10 |
| 4 — Stock operations with a transaction | 20 |
| 5 — Reports | 20 |
| 6 — DataSet feature | 10 |
| 7 — Robustness and README | 10 |

**Automatic deductions:**

| Violation | Deduction |
|-----------|-----------|
| Dapper or EF Core used instead of ADO.NET | −40 |
| Any value put into SQL by concatenation or interpolation | −20 |
| The project does not build from a clean clone | −30 |

---

### Submission

- A Git repository named `minishop-<lastname>` (or a zip with the same name).
- It must run against a **fresh, empty** `minishop` database, using only the app (schema and seed created by code).

---
---

<a id="ru"></a>
## 🇷🇺 Русский

**Формат:** практический, индивидуальный · **Всего:** 100 баллов
**Стек:** консольное приложение .NET · `Npgsql` · `System.Data` · PostgreSQL
**Разрешено:** официальная документация и ваш собственный проект BookNest.
**Запрещено:** списывать у другого студента, ИИ-ассистенты, Dapper, EF Core.

Экзамен проходит на **новой предметной области** — не BookNest. Вы доказываете, что можете построить
ADO.NET-приложение с нуля, от подключения до итогового отчёта.

---

### Сценарий — MiniShop

Создайте консольное приложение для управления категориями, товарами и движением склада небольшого магазина.
Создайте базу данных `minishop`.

**Обязательная схема** (создаётся из кода, безопасно для повторного запуска):
```sql
categories (
    id    SERIAL PRIMARY KEY,
    name  VARCHAR(100) NOT NULL UNIQUE
)

products (
    id           SERIAL PRIMARY KEY,
    name         VARCHAR(150) NOT NULL,
    price        NUMERIC(10,2) NOT NULL CHECK (price > 0),
    quantity     INT NOT NULL DEFAULT 0 CHECK (quantity >= 0),
    category_id  INT NOT NULL REFERENCES categories(id)
)

stock_movements (
    id          SERIAL PRIMARY KEY,
    product_id  INT NOT NULL REFERENCES products(id),
    change_qty  INT NOT NULL CHECK (change_qty <> 0),   -- положительное = поступление, отрицательное = продажа
    moved_at    TIMESTAMP NOT NULL DEFAULT NOW()
)
```

---

### Задание 1 — Структура, подключение и начальные данные (10 баллов)

- Разложите проект по папкам: `Models/`, `Data/`, `Repositories/`, `Services/`, `UI/`.
- Строка подключения находится в **одном** месте — во вспомогательном классе `Database`, который возвращает подключение.
- При запуске проверьте подключение (при ошибке корректно завершитесь с сообщением), создайте схему
  и добавьте **3 категории** и **6 товаров**.
- Повторный запуск приложения **не** должен создавать дубликаты.
- В `Program.cs` нет SQL.

---

### Задание 2 — CRUD товаров и категорий (20 баллов)

**`ProductRepository`:**

| Метод | Возвращает | Описание |
|-------|------------|----------|
| `GetAll()` | `List<Product>` | Все товары |
| `GetById(int id)` | `Product?` | Один товар или `null` |
| `Add(Product p)` | `int` | Добавляет и возвращает новый id |
| `Update(Product p)` | `bool` | `true`, если строка изменена |
| `Delete(int id)` | `bool` | `true`, если строка удалена |

**`CategoryRepository`:** те же пять методов для категорий.

**Требования:**
- Каждая команда параметризована.
- Валидация перед сохранением: название обязательно, цена больше 0, количество 0 или больше. Выведите все ошибки.
- Добавление категории с существующим названием выводит `Category already exists.`
- Удаление категории, в которой ещё есть товары, выводит `Cannot delete: category has products.`
- Оба сообщения получаются перехватом `PostgresException` и проверкой его `SqlState`.
- Меню позволяет просматривать, искать, добавлять, обновлять и удалять товары и категории.

---

### Задание 3 — Поиск и фильтры (10 баллов)

Добавьте в `ProductRepository`:

| Метод | Описание |
|-------|----------|
| `SearchByName(string term)` | Без учёта регистра, по части названия |
| `Filter(int? categoryId, decimal? minPrice, decimal? maxPrice)` | В `WHERE` попадают **только** заданные фильтры |

**Требования:**
- Параметризовано. Поиск `'; DROP TABLE products; --` ничего не возвращает и ничего не меняет.
- Поиск и добавление товара с названием `O'Brien's Cable` работает корректно.

---

### Задание 4 — Операции склада с транзакцией (20 баллов)

Создайте `StockService` с двумя методами.

**`RegisterSale(int productId, int qty)`**

| Ситуация | Сообщение |
|----------|-----------|
| `qty` равно 0 или отрицательное | `Quantity must be greater than 0.` |
| Товар не найден | `Product not found.` |
| `qty` больше остатка | `Cannot sell: only {n} in stock.` |
| Успех | `Sale registered. Remaining: {n}` |

**`Restock(int productId, int qty)`**

| Ситуация | Сообщение |
|----------|-----------|
| `qty` равно 0 или отрицательное | `Quantity must be greater than 0.` |
| Товар не найден | `Product not found.` |
| Успех | `Restocked. New quantity: {n}` |

**Требования:**
- В каждой операции вставка движения и обновление количества выполняются в **одной транзакции**
  на **одном подключении**. Если что-то не удалось, ничего не сохраняется.
- Отклонённая продажа ничего не меняет в базе.
- Добавьте пункты меню для обеих операций.

---

### Задание 5 — Отчёты (20 баллов)

Каждый отчёт — **один SQL-запрос**, объединения и группировка выполняются в SQL, а не в C#.

| Отчёт | Описание |
|-------|----------|
| Products with category | Название, цена, количество и название категории для каждого товара |
| Low stock | Товары с `quantity` ниже порога, введённого пользователем (параметризовано) |
| Category summary | Для каждой категории: количество товаров и общая стоимость остатка (`quantity × price`). Категории **без товаров** должны отображаться с `0` |
| Sales per product | Всего продано единиц по каждому товару, сначала самые продаваемые. Товары **без продаж** должны отображаться с `0` |

**Ожидаемый вывод (пример):**
```
Category summary:
Accessories — 3 product(s) — stock value: 412.50
Cables      — 2 product(s) — stock value: 96.00
Spare parts — 0 product(s) — stock value: 0.00
```

---

### Задание 6 — Функция на DataSet (10 баллов)

1. Загрузите все товары в `DataSet` через `NpgsqlDataAdapter`. Используйте команду выборки по одной таблице
   и `NpgsqlCommandBuilder`.
2. Запросите у пользователя категорию. В памяти увеличьте цену **каждого товара этой категории** на 10%
   (с округлением до 2 знаков).
3. Выведите, сколько строк ожидают сохранения как Modified.
4. Сохраните **одним** `adapter.Update`.
5. Докажите, что это сработало: загрузите товары в **новый** `DataSet` и выведите новые цены.

---

### Задание 7 — Надёжность и README (10 баллов)

- Нет падений при нечисловом вводе, неизвестных id, пустом вводе или потере подключения во время команды.
- `README.md` объясняет: как создать базу данных, как запустить, список функций меню и схему.
- Проект очищен: нет мёртвого или закомментированного кода, в репозитории нет `bin/` и `obj/`.

---

### Самопроверка — что запустит проверяющий

| # | Действие | Ожидается |
|---|----------|-----------|
| 1 | Запустить приложение на пустой базе | Схема создана, начальные данные добавлены, без ошибок |
| 2 | Запустить его ещё раз | Нет дубликатов категорий и товаров |
| 3 | Добавить товар `O'Brien's Cable` | Сохранён |
| 4 | Найти `'; DROP TABLE products; --` | Ничего не найдено; таблица `products` на месте |
| 5 | Продать больше единиц, чем есть на складе | Отклонено; количество и движения не изменились |
| 6 | Продать допустимое количество | Успех; количество уменьшилось; ровно одно новое движение |
| 7 | Удалить категорию, в которой есть товары | `Cannot delete: category has products.` |
| 8 | Открыть все 4 отчёта | Корректные данные; пустая категория и непроданные товары отображаются с `0` |
| 9 | DataSet: поднять цены на 10% в одной категории | Показано число ожидающих; один `Update`; новые цены после перезагрузки |

---

### Оценивание (100 баллов)

| Задание | Баллы |
|---------|-------|
| 1 — Структура, подключение, начальные данные | 10 |
| 2 — CRUD | 20 |
| 3 — Поиск и фильтры | 10 |
| 4 — Операции склада с транзакцией | 20 |
| 5 — Отчёты | 20 |
| 6 — Функция на DataSet | 10 |
| 7 — Надёжность и README | 10 |

**Автоматические вычеты:**

| Нарушение | Вычет |
|-----------|-------|
| Использован Dapper или EF Core вместо ADO.NET | −40 |
| Любое значение подставлено в SQL конкатенацией или интерполяцией | −20 |
| Проект не собирается из чистого клона | −30 |

---

### Сдача

- Git-репозиторий с именем `minishop-<фамилия>` (или zip с таким же именем).
- Он должен работать на **новой, пустой** базе `minishop`, используя только само приложение (схема и начальные данные создаются кодом).

---
---

<a id="tj"></a>
## 🇹🇯 Тоҷикӣ

**Формат:** амалӣ, инфиродӣ · **Ҳамагӣ:** 100 хол
**Технологияҳо:** барномаи консолии .NET · `Npgsql` · `System.Data` · PostgreSQL
**Иҷозат дода шудааст:** ҳуҷҷатҳои расмӣ ва лоиҳаи худи шумо BookNest.
**Иҷозат дода намешавад:** нусха бардоштан аз донишҷӯи дигар, ёрдамчиёни ИИ, Dapper, EF Core.

Имтиҳон дар **соҳаи нав** гузаронида мешавад — на BookNest. Шумо исбот мекунед, ки метавонед барномаи
ADO.NET-ро аз сифр, аз пайвастшавӣ то ҳисоботи ниҳоӣ созед.

---

### Сенария — MiniShop

Барномаи консолӣ барои идоракунии категорияҳо, маҳсулот ва ҳаракати анбори мағозаи хурд созед.
Базаи маълумоти `minishop`-ро эҷод кунед.

**Схемаи ҳатмӣ** (аз код эҷод мешавад, такрор иҷро кардан бехатар):
```sql
categories (
    id    SERIAL PRIMARY KEY,
    name  VARCHAR(100) NOT NULL UNIQUE
)

products (
    id           SERIAL PRIMARY KEY,
    name         VARCHAR(150) NOT NULL,
    price        NUMERIC(10,2) NOT NULL CHECK (price > 0),
    quantity     INT NOT NULL DEFAULT 0 CHECK (quantity >= 0),
    category_id  INT NOT NULL REFERENCES categories(id)
)

stock_movements (
    id          SERIAL PRIMARY KEY,
    product_id  INT NOT NULL REFERENCES products(id),
    change_qty  INT NOT NULL CHECK (change_qty <> 0),   -- мусбат = воридот, манфӣ = фурӯш
    moved_at    TIMESTAMP NOT NULL DEFAULT NOW()
)
```

---

### Супориши 1 — Сохтор, пайвастшавӣ ва маълумоти ибтидоӣ (10 хол)

- Лоиҳаро ба папкаҳо ташкил кунед: `Models/`, `Data/`, `Repositories/`, `Services/`, `UI/`.
- Сатри пайвастшавӣ дар **як** ҷост — дар синфи ёрирасони `Database`, ки пайвастшавӣ бармегардонад.
- Ҳангоми оғоз пайвастшавиро санҷед (ҳангоми хато бо паём дуруст қатъ шавед), схемаро эҷод кунед
  ва **3 категория** ва **6 маҳсулот** илова кунед.
- Иҷрои такрории барнома **набояд** такрорӣ эҷод кунад.
- Дар `Program.cs` SQL нест.

---

### Супориши 2 — CRUD-и маҳсулот ва категорияҳо (20 хол)

**`ProductRepository`:**

| Метод | Бармегардонад | Тавсиф |
|-------|---------------|--------|
| `GetAll()` | `List<Product>` | Ҳамаи маҳсулот |
| `GetById(int id)` | `Product?` | Як маҳсулот ё `null` |
| `Add(Product p)` | `int` | Илова мекунад ва id-и навро бармегардонад |
| `Update(Product p)` | `bool` | `true`, агар сатр тағйир ёфта бошад |
| `Delete(int id)` | `bool` | `true`, агар сатр нест шуда бошад |

**`CategoryRepository`:** ҳамон панҷ метод барои категорияҳо.

**Талабот:**
- Ҳар як фармон параметрдор аст.
- Пеш аз сабт валидатсия: ном ҳатмӣ, нарх аз 0 зиёд, миқдор 0 ё зиёдтар. Ҳамаи хатоҳоро чоп кунед.
- Илова кардани категория бо номи мавҷуда `Category already exists.`-ро чоп мекунад.
- Нест кардани категорияе, ки ҳанӯз маҳсулот дорад, `Cannot delete: category has products.`-ро чоп мекунад.
- Ҳар ду паём бо дастгир кардани `PostgresException` ва санҷиши `SqlState`-и он ба даст меоянд.
- Меню имкон медиҳад, ки маҳсулот ва категорияҳо рӯйхат, ҷустуҷӯ, илова, навсозӣ ва нест карда шаванд.

---

### Супориши 3 — Ҷустуҷӯ ва филтрҳо (10 хол)

Ба `ProductRepository` илова кунед:

| Метод | Тавсиф |
|-------|--------|
| `SearchByName(string term)` | Бе назардошти ҳарфи калон/хурд, аз рӯи қисми ном |
| `Filter(int? categoryId, decimal? minPrice, decimal? maxPrice)` | Дар `WHERE` **танҳо** филтрҳои додашуда ворид мешаванд |

**Талабот:**
- Параметрдор. Ҷустуҷӯи `'; DROP TABLE products; --` чизе барнагардонад ва чизеро тағйир надиҳад.
- Ҷустуҷӯ ва илова кардани маҳсулот бо номи `O'Brien's Cable` дуруст кор мекунад.

---

### Супориши 4 — Амалиётҳои анбор бо транзаксия (20 хол)

`StockService`-ро бо ду метод эҷод кунед.

**`RegisterSale(int productId, int qty)`**

| Ҳолат | Паём |
|-------|------|
| `qty` баробари 0 ё манфӣ | `Quantity must be greater than 0.` |
| Маҳсулот ёфт нашуд | `Product not found.` |
| `qty` аз боқимонда зиёд | `Cannot sell: only {n} in stock.` |
| Муваффақият | `Sale registered. Remaining: {n}` |

**`Restock(int productId, int qty)`**

| Ҳолат | Паём |
|-------|------|
| `qty` баробари 0 ё манфӣ | `Quantity must be greater than 0.` |
| Маҳсулот ёфт нашуд | `Product not found.` |
| Муваффақият | `Restocked. New quantity: {n}` |

**Талабот:**
- Дар ҳар амалиёт вставкаи ҳаракат ва навсозии миқдор дар **як транзаксия**
  бо **як** пайвастшавӣ иҷро мешавад. Агар чизе хато шавад, ҳеҷ чиз сабт намешавад.
- Фурӯши рад шуда дар база ҳеҷ чизро тағйир намедиҳад.
- Барои ҳар ду амалиёт бандҳои меню илова кунед.

---

### Супориши 5 — Ҳисоботҳо (20 хол)

Ҳар ҳисобот — **як дархости SQL** аст, пайвастсозӣ ва гурӯҳбандӣ дар SQL иҷро мешавад, на дар C#.

| Ҳисобот | Тавсиф |
|---------|--------|
| Products with category | Ном, нарх, миқдор ва номи категория барои ҳар маҳсулот |
| Low stock | Маҳсулот бо `quantity` аз ҳадди аз ҷониби корбар воридшуда камтар (параметрдор) |
| Category summary | Барои ҳар категория: шумораи маҳсулот ва арзиши умумии боқимонда (`quantity × price`). Категорияҳои **бе маҳсулот** бояд бо `0` нишон дода шаванд |
| Sales per product | Шумораи умумии воҳидҳои фурӯхташуда барои ҳар маҳсулот, аввал бештарин. Маҳсулоти **бе фурӯш** бояд бо `0` нишон дода шавад |

**Натиҷаи интизоршаванда (мисол):**
```
Category summary:
Accessories — 3 product(s) — stock value: 412.50
Cables      — 2 product(s) — stock value: 96.00
Spare parts — 0 product(s) — stock value: 0.00
```

---

### Супориши 6 — Функсия бо DataSet (10 хол)

1. Ҳамаи маҳсулотро бо `NpgsqlDataAdapter` ба `DataSet` бор кунед. Фармони интихобро аз як ҷадвал ва
   `NpgsqlCommandBuilder`-ро истифода баред.
2. Аз корбар категорияро пурсед. Дар хотира нархи **ҳар як маҳсулоти он категорияро** 10% зиёд кунед
   (бо мудаввар кардан то 2 рақами ҳамсола).
3. Чоп кунед, ки чанд сатр ҳамчун Modified интизори сабт аст.
4. Бо **як** `adapter.Update` сабт кунед.
5. Исбот кунед, ки кор кард: маҳсулотро ба `DataSet`-и **нав** бор кунед ва нархҳои навро чоп кунед.

---

### Супориши 7 — Устувории барнома ва README (10 хол)

- Ҳангоми воридкунии ғайрирақамӣ, id-ҳои номаълум, воридкунии холӣ ё қатъ шудани пайвастшавӣ дар вақти фармон барнома қатъ намешавад.
- `README.md` шарҳ медиҳад: базаро чӣ тавр эҷод кардан, чӣ тавр иҷро кардан, рӯйхати функсияҳои меню ва схема.
- Лоиҳа тоза карда шудааст: коди мурда ё ба шарҳ гирифташуда нест, дар репозиторий `bin/` ва `obj/` нест.

---

### Худсанҷӣ — санҷандагон чӣ иҷро мекунанд

| # | Амал | Интизорӣ |
|---|------|----------|
| 1 | Барномаро дар базаи холӣ оғоз кунед | Схема эҷод шуд, маълумоти ибтидоӣ илова шуд, бе хато |
| 2 | Онро боз оғоз кунед | Категорияҳо ва маҳсулоти такрорӣ нест |
| 3 | Маҳсулоти `O'Brien's Cable`-ро илова кунед | Сабт шуд |
| 4 | `'; DROP TABLE products; --`-ро ҷустуҷӯ кунед | Чизе ёфт нашуд; ҷадвали `products` ҷои худро дорад |
| 5 | Аз миқдори дар анбор буда зиёдтар фурӯшед | Рад шуд; миқдор ва ҳаракатҳо тағйир наёфтанд |
| 6 | Миқдори дурустро фурӯшед | Муваффақият; миқдор кам шуд; дақиқан як ҳаракати нав |
| 7 | Категорияеро, ки маҳсулот дорад, нест кунед | `Cannot delete: category has products.` |
| 8 | Ҳамаи 4 ҳисоботро кушоед | Маълумоти дуруст; категорияи холӣ ва маҳсулоти нафурӯхташуда бо `0` нишон дода мешаванд |
| 9 | DataSet: нархҳоро дар як категория 10% зиёд кунед | Шумораи интизор нишон дода шуд; як `Update`; нархҳои нав баъд аз аз нав бор кардан |

---

### Баҳогузорӣ (100 хол)

| Супориш | Хол |
|---------|-----|
| 1 — Сохтор, пайвастшавӣ, маълумоти ибтидоӣ | 10 |
| 2 — CRUD | 20 |
| 3 — Ҷустуҷӯ ва филтрҳо | 10 |
| 4 — Амалиётҳои анбор бо транзаксия | 20 |
| 5 — Ҳисоботҳо | 20 |
| 6 — Функсия бо DataSet | 10 |
| 7 — Устувории барнома ва README | 10 |

**Тарҳи автоматӣ:**

| Вайронкунӣ | Тарҳ |
|------------|------|
| Ба ҷои ADO.NET Dapper ё EF Core истифода шудааст | −40 |
| Ягон қимат ба SQL бо пайвасткунӣ ё интерполяция гузошта шудааст | −20 |
| Лоиҳа аз clone-и тоза сохта намешавад | −30 |

---

### Супоридан

- Репозиторийи Git бо номи `minishop-<насаб>` (ё zip бо ҳамин ном).
- Он бояд бо базаи **нав ва холии** `minishop` кор кунад, танҳо худи барномаро истифода бурда (схема ва маълумоти ибтидоӣ аз код эҷод мешаванд).
