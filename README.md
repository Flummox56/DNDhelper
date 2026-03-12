# DNDhelper API

## Единая точка входа
```
http://localhost/api/...
```

## Swagger документация
- Auth: http://localhost/swagger/gateway
- Monsters: http://localhost/swagger/monsters

## Эндпоинты

### Auth Service (`/api/auth`)
```
POST   /register     - {username, email, password}
POST   /login        - {username, password}
GET    /profile      - информация о текущем пользователе
POST   /logout       - выход
```

### Monsters Service (`/api/monsters`)
```
GET    /             - список всех монстров
GET    /{id}         - монстр по ID
POST   /             - создать монстра
PATCH  /{id}         - обновить монстра
DELETE /{id}         - удалить монстра
```

## Модель монстра

### Поля для создания (POST/PATCH)
```json
{
  "name": "Гоблин",              // string, обязательное, уникальное для пользователя
  "maxHP": 7,                    // integer
  "ac": 15,                      // integer
  "str": 8,                      // integer
  "dex": 14,                     // integer
  "con": 10,                     // integer
  "int": 8,                      // integer
  "wis": 8,                      // integer
  "cha": 8,                      // integer
  "danger": 0.25,                // number (0.25, 0.5, 0.75, 1.0, 1.5, etc.)
  "description": "Зеленый",      // string
  "status": "private"            // "private" или "public"
}
```

### Ответ сервера (GET, POST, PATCH)
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Гоблин",
  "maxHP": 7,
  "ac": 15,
  "str": 8,
  "dex": 14,
  "con": 10,
  "int": 8,
  "wis": 8,
  "cha": 8,
  "danger": 0.25,
  "experience": 50,
  "description": "Зеленый",
  "createdBy": "user-123",
  "createdByUsername": "testuser",
  "status": "private",
  "createdAt": "2024-03-12T10:30:00Z",
  "updatedAt": "2024-03-12T10:30:00Z"
}
```

## Примеры запросов (JavaScript)

```javascript
// ВАЖНО: всегда добавляйте credentials: 'include'
const baseUrl = 'http://localhost/api';

// 1. Регистрация
await fetch(`${baseUrl}/auth/register`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  credentials: 'include',
  body: JSON.stringify({
    username: 'testuser',
    email: 'test@example.com',
    password: 'password123'
  })
});

// 2. Вход
await fetch(`${baseUrl}/auth/login`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  credentials: 'include',
  body: JSON.stringify({
    username: 'testuser',
    password: 'password123'
  })
});

// 3. Создание монстра
await fetch(`${baseUrl}/monsters`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  credentials: 'include',
  body: JSON.stringify({
    name: "Гоблин",
    maxHP: 7,
    ac: 15,
    str: 8,
    dex: 14,
    con: 10,
    int: 8,
    wis: 8,
    cha: 8,
    danger: 0.25,
    description: "Маленький зеленый",
    status: "public"
  })
});

// 4. Получение списка
await fetch(`${baseUrl}/monsters`, {
  credentials: 'include'
});

// 5. Обновление монстра
await fetch(`${baseUrl}/monsters/123e4567-e89b-12d3-a456-426614174000`, {
  method: 'PATCH',
  headers: { 'Content-Type': 'application/json' },
  credentials: 'include',
  body: JSON.stringify({
    name: "Гоблин-воин",
    maxHP: 15,
    ac: 17,
    danger: 0.5,
    status: "private"
    // остальные поля опциональны
  })
});

// 6. Удаление монстра
await fetch(`${baseUrl}/monsters/123e4567-e89b-12d3-a456-426614174000`, {
  method: 'DELETE',
  credentials: 'include'
});
```

## Коды ответов

| Код | Описание |
|-----|----------|
| `200` | Успех (GET, PATCH) |
| `201` | Создано (POST) |
| `204` | Удалено (DELETE) |
| `400` | Неверный запрос |
| `401` | Не авторизован |
| `403` | Нет прав (не ваш монстр) |
| `404` | Не найдено |
| `409` | Конфликт (монстр с таким именем уже существует) |

## Проверка контейнеров

```bash
# Все контейнеры
docker ps

# Логи конкретного сервиса
docker compose logs -f sheet-service
docker compose logs -f api-gateway-service
docker compose logs -f nginx

# Подключение к БД монстров
docker exec -it sheet_storage psql -U sheet_user -d monster_db
```

## Главные правила
1. **Все запросы** с `credentials: 'include'`
2. **Базовый URL**: `http://localhost/api/...`
3. **POST/PATCH** всегда с Content-Type: application/json
4. **Имена монстров** уникальны для каждого пользователя
5. **danger** принимает дробные числа (0.25, 0.5, 1.5 и т.д.)
