# Discord Application Setup

## OAuth2 URLs

### Redirect URLs
- `http://localhost:8080/auth/callback`

### Application URLs
- Application URL: `https://github.com/fckwzrd/Service-Launcher`
- Documentation: `https://github.com/fckwzrd/Service-Launcher/tree/master/docs`

## Important Notes
- Make sure to add these URLs in your Discord Application OAuth2 settings
- The redirect URL must match exactly with what's configured in the application
- Port 8080 is used by default, but can be configured in `app.dev.json` 