# Настройка Discord приложения

## OAuth2 URLs

### URL для перенаправления
- `http://localhost:8080/auth/callback`

### URL приложения
- URL приложения: `https://github.com/fckwzrd/Service-Launcher`
- Документация: `https://github.com/fckwzrd/Service-Launcher/tree/master/docs`

## Важные замечания
- Убедитесь, что добавили эти URL в настройках OAuth2 вашего Discord приложения
- URL для перенаправления должен точно совпадать с настройками в приложении
- По умолчанию используется порт 8080, но его можно изменить в `app.dev.json` 