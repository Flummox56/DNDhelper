
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
POST   /             - создать {name, maxHP, ac, str, dex, con, int, wis, cha, danger, experience, description, status}
PATCH  /{id}         - обновить (поля как при создании)
DELETE /{id}         - удалить
```

## Модель монстра
```json
{
  "id": "uuid",
  "name": "string",
  "maxHP": 0,
  "ac": 0,
  "str": 0,
  "dex": 0,
  "con": 0,
  "int": 0,
  "wis": 0,
  "cha": 0,
  "danger": "low|medium|high|very high",
  "experience": 0,
  "description": "string",
  "createdBy": "user-id",
  "createdByUsername": "string",
  "status": "private|public",
  "createdAt": "2024-03-08T10:30:00Z",
  "updatedAt": "2024-03-08T10:30:00Z"
}
```

## Пример запроса (JavaScript)
```javascript
// Важно: всегда добавляйте credentials: 'include'
fetch('http://localhost/api/monsters', {
  credentials: 'include'
});

// POST с телом
fetch('http://localhost/api/monsters', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  credentials: 'include',
  body: JSON.stringify({
    name: "Дракон",
    maxHP: 95,
    ac: 17,
    str: 18,
    dex: 10,
    con: 16,
    int: 8,
    wis: 12,
    cha: 12,
    danger: "high",
    experience: 2300,
    description: "Описание",
    status: "private"
  })
});
```

## Коды ответов
- `200` - успех
- `201` - создано
- `204` - удалено (без тела)
- `400` - неверный запрос
- `401` - не авторизован
- `403` - нет прав
- `404` - не найдено

## Проверка контейнеров
```bash
docker ps
```

Главное правило: **все запросы с `credentials: 'include'`** и базовый URL `http://localhost/api/...`
