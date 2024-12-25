# Используем официальный Node.js образ
FROM node:18

# Указываем рабочую директорию
WORKDIR /app

# Копируем package.json и package-lock.json
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем все файлы проекта
COPY . .

# Указываем порт для React-приложения
EXPOSE 3000

# Запускаем приложение в режиме разработки
CMD ["npm", "start"]
