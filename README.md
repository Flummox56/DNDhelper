# DNDhelper API

## Технологии

- **.NET 10.0** - backend фреймворк
- **PostgreSQL 16** - база данных
- **Entity Framework Core 10.0** - ORM
- **Docker & Docker Compose** - контейнеризация
- **Swagger/OpenAPI** - документация API

## Требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows/Mac) или Docker Engine (Linux)
- [Git](https://git-scm.com/) (для клонирования репозитория)
- 4-8GB свободной оперативной памяти
- Свободные порты: **8080** (API), **5432** (PostgreSQL)

## Быстрый старт

### 1. Клонирование репозитория
```bash
git clone <url-репозитория>
cd DNDhelper
```

### 2. Запуск через Docker Compose

```bash
# Запуск контейнеров
docker-compose up --build

# Или в фоновом режиме
docker-compose up --build -d
```

### 3. Проверка запуска

```bash
# Проверить, что контейнеры запущены
docker ps

# Должны быть видны:
# - dndhelper-api (порт 8080)
# - auth_postgres (порт 5432)
```

### 4. Доступ к API

- **Swagger UI**: http://localhost:8080/swagger
- **API base URL**: http://localhost:8080

## API Endpoints

### Авторизация (`/api/auth`)

| Метод | Endpoint | Описание | Тело запроса |
|-------|----------|----------|--------------|
| POST | `/register` | Регистрация нового пользователя | `{ "username": "string", "email": "string", "password": "string" }` |
| POST | `/login` | Вход в систему | `{ "username": "string", "password": "string" }` |
| POST | `/logout` | Выход из системы | - |
| GET | `/me` | Информация о текущем пользователе | - |

### Тестовые эндпоинты (`/api/test`)

| Метод | Endpoint | Описание | Доступ |
|-------|----------|----------|--------|
| GET | `/public` | Публичный эндпоинт (без авторизации) | Все |
| GET | `/protected` | Защищенный эндпоинт | Только авторизованные |

## Проверка работы

### Способ 1: Через Swagger UI

1. Откройте http://localhost:8080/swagger
2. Разверните нужный эндпоинт
3. Нажмите "Try it out"
4. Заполните данные и выполните запрос

### Способ 2: Через curl

```bash
# 1. Регистрация
curl -X POST http://localhost:8080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "password123"
  }'

# 2. Вход (сохраняем cookies)
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "password123"
  }' \
  -c cookies.txt

# 3. Проверка авторизации
curl -X GET http://localhost:8080/api/auth/me \
  -b cookies.txt

# 4. Защищенный эндпоинт
curl -X GET http://localhost:8080/api/test/protected \
  -b cookies.txt

# 5. Выход
curl -X POST http://localhost:8080/api/auth/logout \
  -b cookies.txt
```

### Способ 3: Проверка базы данных

```bash
# Подключиться к PostgreSQL
docker exec -it auth_postgres psql -U auth_user -d auth_db

# Посмотреть пользователей
SELECT * FROM "users";

# Посмотреть сессии
SELECT * FROM "sessions";

# Выйти
\q
```

## Структура проекта

```
DNDhelper/
├── DNDhelper/                          # Основной проект
│   ├── Controllers/
│   │   └── AuthController.cs           # Регистрация, логин, logout, me
│   ├── Data/
│   │   └── AuthDbContext.cs            # Контекст базы данных
│   ├── Models/
│   │   ├── User.cs                      # Модель пользователя
│   │   ├── Session.cs                    # Модель сессии
│   │   └── AuthModels.cs                 # Request/Response модели
│   ├── Services/
│   │   └── SessionService.cs             # Сервис для работы с сессиями
│   ├── Migrations/                        # Миграции EF Core
│   ├── appsettings.json
│   ├── Program.cs
│   └── DNDhelper.csproj
├── docker-compose.yml
├── docker-compose.override.yml
└── README.md
```

## Конфигурация

### Переменные окружения (в docker-compose.yml)

```yaml
# PostgreSQL
POSTGRES_DB: auth_db
POSTGRES_USER: auth_user
POSTGRES_PASSWORD: auth_password

# API
ASPNETCORE_ENVIRONMENT: Development
ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=auth_db;Username=auth_user;Password=auth_password
```

## Примечания

1. **Базовый URL API**: `http://localhost:8080`
2. **Авторизация** работает через cookies
3. **Все запросы к защищенным эндпоинтам** должны включать `credentials: 'include'`
4. **Swagger документация** доступна по адресу `/swagger`

### Пример запроса с фронтенда

```javascript
// Регистрация
const register = async () => {
  const response = await fetch('http://localhost:8080/api/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({
      username: 'user123',
      email: 'user@example.com',
      password: 'password123'
    })
  });
  return response.json();
};

// Получение текущего пользователя
const getMe = async () => {
  const response = await fetch('http://localhost:8080/api/auth/me', {
    credentials: 'include'
  });
  return response.json();
};
```
