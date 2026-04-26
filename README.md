# Telegram Chat Bot

It is just telegram chatbot which help user to get cat's images and talk with chat assistant

# Deploying

## Docker

```shell
nano docker-compose.yml # define necessary env variables
docker compose up -d -f docker-compose.yml
```

# Demo

You can try this bot on the [link](https://t.me/chatcattybot)

# Environment variables

| variable              | meaning                                  | necessary | Default value               |
|-----------------------|------------------------------------------|-----------|-----------------------------|
| `TELEGRAM_BOT_TOKEN`  | Telegram bot token from @BotFather       | true      |                             |
| `OPENAI_TOKEN`        | Open AI token                            | true      |                             |
| `GOOGLE_API_KEY`      | Gemini API token                         | true      |                             |
| `ADMIN_TG_IDS`        | Telegram user id list of admins          | false     | null                        |
| `CATTY_PS_USERNAME`   | Postgres username to save statistic      | false     | postgres                    |
| `CATTY_PS_PASSWORD`   | Postgres password to save statistic      | false     | Empty password              |
| `CATTY_PS_HOSTNAME`   | Postgres hostname to save statistic      | false     | localhost                   |
| `CATTY_PS_DATABASE`   | Postgres database name to save statistic | false     | same as `CATTY_PS_USERNAME` |
| `CATTY_LOG_DIRECTORY` | path to log files                        | false     | null                        |
| `CHATS_WHITELIST`     | Allowed telegram chats ids               | false     | All chats                   |

# TODO
- use Scrutor
- User-defined prompts
