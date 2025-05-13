using System.Collections.Generic;

namespace _123123.Services
{
    public enum Language
    {
        English,
        Russian,
        Chinese,
        Portuguese,
        Spanish,
        French,
        German
    }

    public class LanguageService
    {
        private static LanguageService _instance;
        private Language _currentLanguage = Language.English;

        private readonly Dictionary<string, Dictionary<Language, string>> _messages = new Dictionary<string, Dictionary<Language, string>>
        {
            {
                "LoginTooltipHeader", new Dictionary<Language, string>
                {
                    { Language.English, "Login Requirements" },
                    { Language.Russian, "Требования к логину" },
                    { Language.Chinese, "登录要求" },
                    { Language.Portuguese, "Requisitos de Login" },
                    { Language.Spanish, "Requisitos de Usuario" },
                    { Language.French, "Exigences de Connexion" },
                    { Language.German, "Login-Anforderungen" }
                }
            },
            {
                "LoginTooltipText", new Dictionary<Language, string>
                {
                    { Language.English, "• 4-15 characters\n• Latin letters and numbers only\n• Cannot be numbers only" },
                    { Language.Russian, "• 4-15 символов\n• Только латинские буквы и цифры\n• Не может состоять только из цифр" },
                    { Language.Chinese, "• 4-15个字符\n• 仅限拉丁字母和数字\n• 不能仅包含数字" },
                    { Language.Portuguese, "• 4-15 caracteres\n• Apenas letras latinas e números\n• Não pode conter apenas números" },
                    { Language.Spanish, "• 4-15 caracteres\n• Solo letras latinas y números\n• No puede contener solo números" },
                    { Language.French, "• 4-15 caractères\n• Lettres latines et chiffres uniquement\n• Ne peut pas contenir que des chiffres" },
                    { Language.German, "• 4-15 Zeichen\n• Nur lateinische Buchstaben und Zahlen\n• Darf nicht nur aus Zahlen bestehen" }
                }
            },
            {
                "PasswordTooltipHeader", new Dictionary<Language, string>
                {
                    { Language.English, "Password Requirements" },
                    { Language.Russian, "Требования к паролю" },
                    { Language.Chinese, "密码要求" },
                    { Language.Portuguese, "Requisitos de Senha" },
                    { Language.Spanish, "Requisitos de Contraseña" },
                    { Language.French, "Exigences du Mot de Passe" },
                    { Language.German, "Passwort-Anforderungen" }
                }
            },
            {
                "PasswordTooltipText", new Dictionary<Language, string>
                {
                    { Language.English, "• Minimum 6 characters\n• One uppercase letter (A-Z)\n• One lowercase letter (a-z)\n• One number\n• One special character: @ - $ ! % * # ? &" },
                    { Language.Russian, "• Минимум 6 символов\n• Одна заглавная буква (A-Z)\n• Одна строчная буква (a-z)\n• Одна цифра\n• Один спецсимвол: @ - $ ! % * # ? &" },
                    { Language.Chinese, "• 至少6个字符\n• 一个大写字母 (A-Z)\n• 一个小写字母 (a-z)\n• 一个数字\n• 一个特殊字符: @ - $ ! % * # ? &" },
                    { Language.Portuguese, "• Mínimo 6 caracteres\n• Uma letra maiúscula (A-Z)\n• Uma letra minúscula (a-z)\n• Um número\n• Um caractere especial: @ - $ ! % * # ? &" },
                    { Language.Spanish, "• Mínimo 6 caracteres\n• Una letra mayúscula (A-Z)\n• Una letra minúscula (a-z)\n• Un número\n• Un carácter especial: @ - $ ! % * # ? &" },
                    { Language.French, "• Minimum 6 caractères\n• Une lettre majuscule (A-Z)\n• Une lettre minuscule (a-z)\n• Un chiffre\n• Un caractère spécial: @ - $ ! % * # ? &" },
                    { Language.German, "• Mindestens 6 Zeichen\n• Ein Großbuchstabe (A-Z)\n• Ein Kleinbuchstabe (a-z)\n• Eine Zahl\n• Ein Sonderzeichen: @ - $ ! % * # ? &" }
                }
            },
            {
                "FillAllFields", new Dictionary<Language, string>
                {
                    { Language.English, "Please fill in all fields" },
                    { Language.Russian, "Пожалуйста, заполните все поля" },
                    { Language.Chinese, "请填写所有字段" },
                    { Language.Portuguese, "Por favor, preencha todos os campos" },
                    { Language.Spanish, "Por favor, complete todos los campos" },
                    { Language.French, "Veuillez remplir tous les champs" },
                    { Language.German, "Bitte füllen Sie alle Felder aus" }
                }
            },
            {
                "InvalidUsername", new Dictionary<Language, string>
                {
                    { Language.English, "Username must be 4-15 characters long, contain only Latin letters and numbers, cannot contain special characters, and cannot be numbers only" },
                    { Language.Russian, "Логин должен содержать от 4 до 15 символов, только латинские буквы и цифры, не может содержать спецсимволы и не может состоять только из цифр" },
                    { Language.Chinese, "用户名必须包含4-15个字符，只能包含拉丁字母和数字，不能包含特殊字符，且不能仅包含数字" },
                    { Language.Portuguese, "O nome de usuário deve ter entre 4 e 15 caracteres, conter apenas letras latinas e números, não pode conter caracteres especiais e não pode conter apenas números" },
                    { Language.Spanish, "El nombre de usuario debe tener entre 4 y 15 caracteres, contener solo letras latinas y números, no puede contener caracteres especiales y no puede contener solo números" },
                    { Language.French, "Le nom d'utilisateur doit contenir entre 4 et 15 caractères, uniquement des lettres latines et des chiffres, ne peut pas contenir de caractères spéciaux et ne peut pas contenir que des chiffres" },
                    { Language.German, "Der Benutzername muss zwischen 4 und 15 Zeichen lang sein, darf nur lateinische Buchstaben und Zahlen enthalten, keine Sonderzeichen und darf nicht nur aus Zahlen bestehen" }
                }
            },
            {
                "InvalidEmail", new Dictionary<Language, string>
                {
                    { Language.English, "Please enter a valid email address" },
                    { Language.Russian, "Пожалуйста, введите корректный email адрес" },
                    { Language.Chinese, "请输入有效的电子邮件地址" },
                    { Language.Portuguese, "Por favor, insira um endereço de email válido" },
                    { Language.Spanish, "Por favor, introduzca una dirección de correo electrónico válida" },
                    { Language.French, "Veuillez entrer une adresse e-mail valide" },
                    { Language.German, "Bitte geben Sie eine gültige E-Mail-Adresse ein" }
                }
            },
            {
                "InvalidPassword", new Dictionary<Language, string>
                {
                    { Language.English, "Password must contain at least 6 characters, one uppercase letter (A-Z), one lowercase letter (a-z), one number and one special character" },
                    { Language.Russian, "Пароль должен содержать минимум 6 символов, одну заглавную букву (A-Z), одну строчную букву (a-z), одну цифру и один спецсимвол:" },
                    { Language.Chinese, "密码必须包含至少6个字符，一个大写字母(A-Z)，一个小写字母(a-z)，一个数字和一个特殊字符" },
                    { Language.Portuguese, "A senha deve conter pelo menos 6 caracteres, uma letra maiúscula (A-Z), uma letra minúscula (a-z), um número e um caractere especial" },
                    { Language.Spanish, "La contraseña debe contener al menos 6 caracteres, una letra mayúscula (A-Z), una letra minúscula (a-z), un número y un carácter especial" },
                    { Language.French, "Le mot de passe doit contenir au moins 6 caractères, une majuscule (A-Z), une minuscule (a-z), un chiffre et un caractère spécial" },
                    { Language.German, "Das Passwort muss mindestens 6 Zeichen, einen Großbuchstaben (A-Z), einen Kleinbuchstaben (a-z), eine Zahl und ein Sonderzeichen enthalten" }
                }
            },
            {
                "PasswordsDoNotMatch", new Dictionary<Language, string>
                {
                    { Language.English, "Passwords do not match" },
                    { Language.Russian, "Пароли не совпадают" },
                    { Language.Chinese, "密码不匹配" },
                    { Language.Portuguese, "As senhas não coincidem" },
                    { Language.Spanish, "Las contraseñas no coinciden" },
                    { Language.French, "Les mots de passe ne correspondent pas" },
                    { Language.German, "Die Passwörter stimmen nicht überein" }
                }
            },
            {
                "EmailTaken", new Dictionary<Language, string>
                {
                    { Language.English, "This email is already registered" },
                    { Language.Russian, "Этот email уже зарегистрирован" },
                    { Language.Chinese, "该电子邮件已注册" },
                    { Language.Portuguese, "Este email já está registrado" },
                    { Language.Spanish, "Este correo electrónico ya está registrado" },
                    { Language.French, "Cet e-mail est déjà enregistré" },
                    { Language.German, "Diese E-Mail ist bereits registriert" }
                }
            },
            {
                "UsernameTaken", new Dictionary<Language, string>
                {
                    { Language.English, "Username already exists" },
                    { Language.Russian, "Такой логин уже существует" },
                    { Language.Chinese, "用户名已存在" },
                    { Language.Portuguese, "Nome de usuário já existe" },
                    { Language.Spanish, "El nombre de usuario ya existe" },
                    { Language.French, "Le nom d'utilisateur existe déjà" },
                    { Language.German, "Benutzername existiert bereits" }
                }
            },
            {
                "WelcomeMessage", new Dictionary<Language, string>
                {
                    { Language.English, "Welcome, {0}!" },
                    { Language.Russian, "Добро пожаловать, {0}!" },
                    { Language.Chinese, "欢迎，{0}!" },
                    { Language.Portuguese, "Bem-vindo, {0}!" },
                    { Language.Spanish, "Bienvenido, {0}!" },
                    { Language.French, "Bienvenue, {0}!" },
                    { Language.German, "Willkommen, {0}!" }
                }
            },
            {
                "RegistrationSuccessful", new Dictionary<Language, string>
                {
                    { Language.English, "Registration successful" },
                    { Language.Russian, "Регистрация успешна" },
                    { Language.Chinese, "注册成功" },
                    { Language.Portuguese, "Registro bem-sucedido" },
                    { Language.Spanish, "Registro exitoso" },
                    { Language.French, "Inscription réussie" },
                    { Language.German, "Registrierung erfolgreich" }
                }
            },
            {
                "InvalidCredentials", new Dictionary<Language, string>
                {
                    { Language.English, "Invalid username or password" },
                    { Language.Russian, "Неверный логин или пароль" },
                    { Language.Chinese, "用户名或密码无效" },
                    { Language.Portuguese, "Nome de usuário ou senha inválidos" },
                    { Language.Spanish, "Nombre de usuario o contraseña no válidos" },
                    { Language.French, "Nom d'utilisateur ou mot de passe invalide" },
                    { Language.German, "Ungültiger Benutzername oder Passwort" }
                }
            },
            {
                "LoginTitle", new Dictionary<Language, string>
                {
                    { Language.English, "Sign In" },
                    { Language.Russian, "Вход" },
                    { Language.Chinese, "登录" },
                    { Language.Portuguese, "Entrar" },
                    { Language.Spanish, "Iniciar Sesión" },
                    { Language.French, "Connexion" },
                    { Language.German, "Anmelden" }
                }
            },
            {
                "EmailField", new Dictionary<Language, string>
                {
                    { Language.English, "Email" },
                    { Language.Russian, "Email" },
                    { Language.Chinese, "电子邮件" },
                    { Language.Portuguese, "Email" },
                    { Language.Spanish, "Correo electrónico" },
                    { Language.French, "Email" },
                    { Language.German, "E-Mail" }
                }
            },
            {
                "RegisterTitle", new Dictionary<Language, string>
                {
                    { Language.English, "Sign Up" },
                    { Language.Russian, "Регистрация" },
                    { Language.Chinese, "注册" },
                    { Language.Portuguese, "Cadastro" },
                    { Language.Spanish, "Registrarse" },
                    { Language.French, "Inscription" },
                    { Language.German, "Registrierung" }
                }
            },
            {
                "LoginField", new Dictionary<Language, string>
                {
                    { Language.English, "Login:" },
                    { Language.Russian, "Логин:" },
                    { Language.Chinese, "用户名：" },
                    { Language.Portuguese, "Login:" },
                    { Language.Spanish, "Usuario:" },
                    { Language.French, "Identifiant:" },
                    { Language.German, "Benutzername:" }
                }
            },
            {
                "PasswordField", new Dictionary<Language, string>
                {
                    { Language.English, "Password:" },
                    { Language.Russian, "Пароль:" },
                    { Language.Chinese, "密码：" },
                    { Language.Portuguese, "Senha:" },
                    { Language.Spanish, "Contraseña:" },
                    { Language.French, "Mot de passe:" },
                    { Language.German, "Passwort:" }
                }
            },
            {
                "ConfirmPasswordField", new Dictionary<Language, string>
                {
                    { Language.English, "Confirm password:" },
                    { Language.Russian, "Подтвердите пароль:" },
                    { Language.Chinese, "确认密码：" },
                    { Language.Portuguese, "Confirmar senha:" },
                    { Language.Spanish, "Confirmar contraseña:" },
                    { Language.French, "Confirmer le mot de passe:" },
                    { Language.German, "Passwort bestätigen:" }
                }
            },
            {
                "LoginButton", new Dictionary<Language, string>
                {
                    { Language.English, "Sign In" },
                    { Language.Russian, "Войти" },
                    { Language.Chinese, "登录" },
                    { Language.Portuguese, "Entrar" },
                    { Language.Spanish, "Iniciar Sesión" },
                    { Language.French, "Se Connecter" },
                    { Language.German, "Anmelden" }
                }
            },
            {
                "RegisterButton", new Dictionary<Language, string>
                {
                    { Language.English, "Sign Up" },
                    { Language.Russian, "Зарегистрироваться" },
                    { Language.Chinese, "注册" },
                    { Language.Portuguese, "Cadastrar" },
                    { Language.Spanish, "Registrarse" },
                    { Language.French, "S'inscrire" },
                    { Language.German, "Registrieren" }
                }
            },
            {
                "NoAccount", new Dictionary<Language, string>
                {
                    { Language.English, "Don't have an account?" },
                    { Language.Russian, "Нет аккаунта?" },
                    { Language.Chinese, "还没有账号？" },
                    { Language.Portuguese, "Não tem uma conta?" },
                    { Language.Spanish, "¿No tienes cuenta?" },
                    { Language.French, "Pas de compte ?" },
                    { Language.German, "Noch kein Konto?" }
                }
            },
            {
                "HaveAccount", new Dictionary<Language, string>
                {
                    { Language.English, "Already have an account?" },
                    { Language.Russian, "Уже есть аккаунт?" },
                    { Language.Chinese, "已有账号？" },
                    { Language.Portuguese, "Já tem uma conta?" },
                    { Language.Spanish, "¿Ya tienes cuenta?" },
                    { Language.French, "Déjà un compte ?" },
                    { Language.German, "Bereits ein Konto?" }
                }
            },
            {
                "LoginTooltipMinLength", new Dictionary<Language, string>
                {
                    { Language.English, "• Case sensitive\n• Must be 4-15 characters\n• Only Latin letters and numbers\n• Cannot contain special characters\n• Cannot be numbers only" },
                    { Language.Russian, "• Учитывается регистр\n• От 4 до 15 символов\n• Только латинские буквы и цифры\n• Нельзя использовать спецсимволы\n• Не может состоять только из цифр" },
                    { Language.Chinese, "• 区分大小写\n• 4-15个字符\n• 仅限拉丁字母和数字\n• 不能包含特殊字符\n• 不能仅包含数字" },
                    { Language.Portuguese, "• Sensível a maiúsculas e minúsculas\n• De 4 a 15 caracteres\n• Apenas letras latinas e números\n• Não pode conter caracteres especiais\n• Não pode conter apenas números" },
                    { Language.Spanish, "• Distingue mayúsculas y minúsculas\n• De 4 a 15 caracteres\n• Solo letras latinas y números\n• No puede contener caracteres especiales\n• No puede contener solo números" },
                    { Language.French, "• Sensible à la casse\n• De 4 à 15 caractères\n• Uniquement lettres latines et chiffres\n• Ne peut pas contenir de caractères spéciaux\n• Ne peut pas contenir que des chiffres" },
                    { Language.German, "• Groß-/Kleinschreibung beachten\n• 4-15 Zeichen\n• Nur lateinische Buchstaben und Zahlen\n• Keine Sonderzeichen erlaubt\n• Darf nicht nur aus Zahlen bestehen" }
                }
            },
            {
                "LoginTooltipChars", new Dictionary<Language, string>
                {
                    { Language.English, "• Latin letters and numbers only" },
                    { Language.Russian, "• Латинские буквы и цифры" },
                    { Language.Chinese, "• 仅限拉丁字母和数字" },
                    { Language.Portuguese, "• Apenas letras latinas e números" },
                    { Language.Spanish, "• Solo letras latinas y números" },
                    { Language.French, "• Lettres latines et chiffres uniquement" },
                    { Language.German, "• Nur lateinische Buchstaben und Zahlen" }
                }
            },
            {
                "MustBeUnique", new Dictionary<Language, string>
                {
                    { Language.English, "• Must be unique" },
                    { Language.Russian, "• Должен быть уникальным" },
                    { Language.Chinese, "• 必须是唯一的" },
                    { Language.Portuguese, "• Deve ser único" },
                    { Language.Spanish, "• Debe ser único" },
                    { Language.French, "• Doit être unique" },
                    { Language.German, "• Muss eindeutig sein" }
                }
            },
            {
                "EmailTooltipHeader", new Dictionary<Language, string>
                {
                    { Language.English, "Email Requirements" },
                    { Language.Russian, "Требования к email" },
                    { Language.Chinese, "电子邮件要求" },
                    { Language.Portuguese, "Requisitos de Email" },
                    { Language.Spanish, "Requisitos de Correo Electrónico" },
                    { Language.French, "Exigences de l'Email" },
                    { Language.German, "E-Mail-Anforderungen" }
                }
            },
            {
                "EmailTooltipFormat", new Dictionary<Language, string>
                {
                    { Language.English, "• Format: example@domain.com" },
                    { Language.Russian, "• Формат: example@domain.com" },
                    { Language.Chinese, "• 格式：example@domain.com" },
                    { Language.Portuguese, "• Formato: example@domain.com" },
                    { Language.Spanish, "• Formato: example@domain.com" },
                    { Language.French, "• Format: example@domain.com" },
                    { Language.German, "• Format: example@domain.com" }
                }
            },
            {
                "EmailTooltipValid", new Dictionary<Language, string>
                {
                    { Language.English, "• Must be a valid email address" },
                    { Language.Russian, "• Действующий email адрес" },
                    { Language.Chinese, "• 必须是有效的电子邮件地址" },
                    { Language.Portuguese, "• Deve ser um endereço de email válido" },
                    { Language.Spanish, "• Debe ser una dirección de correo válida" },
                    { Language.French, "• Doit être une adresse email valide" },
                    { Language.German, "• Muss eine gültige E-Mail-Adresse sein" }
                }
            },
            {
                "PasswordTooltipMinLength", new Dictionary<Language, string>
                {
                    { Language.English, "• Minimum 6 characters" },
                    { Language.Russian, "• Минимум 6 символов" },
                    { Language.Chinese, "• 最少6个字符" },
                    { Language.Portuguese, "• Mínimo de 6 caracteres" },
                    { Language.Spanish, "• Mínimo 6 caracteres" },
                    { Language.French, "• Minimum 6 caractères" },
                    { Language.German, "• Mindestens 6 Zeichen" }
                }
            },
            {
                "PasswordTooltipUppercase", new Dictionary<Language, string>
                {
                    { Language.English, "• At least one uppercase letter (A-Z)" },
                    { Language.Russian, "• Минимум одна заглавная буква (A-Z)" },
                    { Language.Chinese, "• 至少一个大写字母 (A-Z)" },
                    { Language.Portuguese, "• Pelo menos uma letra maiúscula (A-Z)" },
                    { Language.Spanish, "• Al menos una letra mayúscula (A-Z)" },
                    { Language.French, "• Au moins une lettre majuscule (A-Z)" },
                    { Language.German, "• Mindestens ein Großbuchstabe (A-Z)" }
                }
            },
            {
                "PasswordTooltipLowercase", new Dictionary<Language, string>
                {
                    { Language.English, "• At least one lowercase letter (a-z)" },
                    { Language.Russian, "• Минимум одна строчная буква (a-z)" },
                    { Language.Chinese, "• 至少一个小写字母 (a-z)" },
                    { Language.Portuguese, "• Pelo menos uma letra minúscula (a-z)" },
                    { Language.Spanish, "• Al menos una letra minúscula (a-z)" },
                    { Language.French, "• Au moins une lettre minuscule (a-z)" },
                    { Language.German, "• Mindestens ein Kleinbuchstabe (a-z)" }
                }
            },
            {
                "PasswordTooltipNumber", new Dictionary<Language, string>
                {
                    { Language.English, "• At least one number (0-9)" },
                    { Language.Russian, "• Минимум одна цифра (0-9)" },
                    { Language.Chinese, "• 至少一个数字 (0-9)" },
                    { Language.Portuguese, "• Pelo menos um número (0-9)" },
                    { Language.Spanish, "• Al menos un número (0-9)" },
                    { Language.French, "• Au moins un chiffre (0-9)" },
                    { Language.German, "• Mindestens eine Zahl (0-9)" }
                }
            },
            {
                "PasswordTooltipSpecial", new Dictionary<Language, string>
                {
                    { Language.English, "• At least one special character: @ - $ ! % * # ? &" },
                    { Language.Russian, "• Минимум один спецсимвол: @ - $ ! % * # ? &" },
                    { Language.Chinese, "• 至少一个特殊字符：@ - $ ! % * # ? &" },
                    { Language.Portuguese, "• Pelo menos um caractere especial: @ - $ ! % * # ? &" },
                    { Language.Spanish, "• Al menos un carácter especial: @ - $ ! % * # ? &" },
                    { Language.French, "• Au moins un caractère spécial : @ - $ ! % * # ? &" },
                    { Language.German, "• Mindestens ein Sonderzeichen: @ - $ ! % * # ? &" }
                }
            },
            {
                "ConfirmPasswordTooltipHeader", new Dictionary<Language, string>
                {
                    { Language.English, "Confirm Password" },
                    { Language.Russian, "Подтверждение пароля" },
                    { Language.Chinese, "确认密码" },
                    { Language.Portuguese, "Confirmar Senha" },
                    { Language.Spanish, "Confirmar Contraseña" },
                    { Language.French, "Confirmer le Mot de Passe" },
                    { Language.German, "Passwort Bestätigen" }
                }
            },
            {
                "ConfirmPasswordTooltipMatch", new Dictionary<Language, string>
                {
                    { Language.English, "• Must match the password entered above" },
                    { Language.Russian, "• Должен совпадать с введённым выше паролем" },
                    { Language.Chinese, "• 必须与上面输入的密码匹配" },
                    { Language.Portuguese, "• Deve corresponder à senha inserida acima" },
                    { Language.Spanish, "• Debe coincidir con la contraseña ingresada arriba" },
                    { Language.French, "• Doit correspondre au mot de passe saisi ci-dessus" },
                    { Language.German, "• Muss mit dem oben eingegebenen Passwort übereinstimmen" }
                }
            }
        };

        private LanguageService() { }

        public static LanguageService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LanguageService();
                }
                return _instance;
            }
        }

        public string GetMessage(string key)
        {
            if (_messages.TryGetValue(key, out var translations) && 
                translations.TryGetValue(_currentLanguage, out var message))
            {
                return message;
            }
            return string.Empty;
        }

        public string GetFormattedMessage(string key, params object[] args)
        {
            string message = GetMessage(key);
            return string.Format(message, args);
        }

        public void SetLanguage(Language language)
        {
            _currentLanguage = language;
        }

        public Language CurrentLanguage => _currentLanguage;

        public static string GetLanguageDisplayName(Language language)
        {
            return language switch
            {
                Language.English => "English",
                Language.Russian => "Русский",
                Language.Chinese => "中文",
                Language.Portuguese => "Português",
                Language.Spanish => "Español",
                Language.French => "Français",
                Language.German => "Deutsch",
                _ => "English"
            };
        }
    }
} 