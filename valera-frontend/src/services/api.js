import axios from 'axios';

const API_BASE_URL = 'http://localhost:5247/api'; // URL вашего backend API

// Создаем axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
});

// Интерceptors для добавления токена к каждому запросу
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Интерceptors для обработки 401 ошибок
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Удаляем токен и перенаправляем на страницу логина
      localStorage.removeItem('token');
      window.location.href = '/login'; // или другая страница логина
    }
    return Promise.reject(error);
  }
);

// Аутентификация
export const register = async (userData) => {
  try {
    const response = await api.post('/auth/register', userData);
    if (response.data.Token) {
      localStorage.setItem('token', response.data.Token);
      localStorage.setItem('user', response.data.User);
    }
    return response.data;
  } catch (error) {
    console.error('Ошибка при регистрации:', error);
    throw error;
  }
};

export const login = async (userData) => {
  try {
    console.log('Отправка запроса на логин...');
    const response = await api.post('/auth/login', userData);
    console.log('Ответ от сервера:', response.data);
    
    if (response.data.Token) {
      console.log('Токен получен, сохраняем...');
      localStorage.setItem('token', response.data.Token);
      localStorage.setItem('user', response.data.User);
      console.log('Токен сохранен в localStorage');
    } else if (response.data.token) { // Проверьте lowercase тоже
      console.log('Токен получен (нижний регистр), сохраняем...');
      localStorage.setItem('token', response.data.token);
      console.log('Токен сохранен в localStorage');
    } else {
      console.warn('Токен не найден в ответе сервера');
    }
    
    return response.data;
  } catch (error) {
    console.error('Ошибка при входе:', error);
    throw error;
  }
};

export const logout = () => {
  localStorage.removeItem('token');
};

export const getCurrentUser = () => {
  const token = localStorage.getItem('token');
  if (!token) {
    console.log('Токен не найден в localStorage');
    return null;
  }

  console.log('Токен найден, длина:', token.length);
  
  try {
    // Более надежный способ декодирования JWT
    const payloadBase64 = token.split('.')[1];
    if (!payloadBase64) {
      console.error('Неверный формат токена');
      return null;
    }
    
    // Декодируем base64
    const payload = JSON.parse(atob(payloadBase64));
    console.log('Декодированный токен:', payload);
    return payload;
    
  } catch (error) {
    console.error('Ошибка при декодировании токена:', error);
    return null;
  }
};

// Получить всех Валер
export const getAllValeras = async () => {
  try {
    const response = await api.get('');
    return response.data;
  } catch (error) {
    console.error('Ошибка при получении списка Валер:', error);
    throw error;
  }
};

// Получить "своих" Валер
export const getMyValeras = async () => {
  try {
    const response = await api.get('/my');
    return response.data;
  } catch (error) {
    console.error('Ошибка при получении списка моих Валер:', error);
    throw error;
  }
};

// Создать нового Валера
export const createValera = async (valeraData) => {
  try {
    const response = await api.post('', valeraData);
    return response.data;
  } catch (error) {
    console.error('Ошибка при создании Валеры:', error);
    throw error;
  }
};

// Получить Валеру по ID
export const getValeraById = async (id) => {
 try {
    const response = await api.get(`/${id}`);
    return response.data;
  } catch (error) {
    console.error('Ошибка при получении Валеры по ID:', error);
    throw error;
 }
};

// Обновить Валеру
export const updateValera = async (id, valeraData) => {
  try {
    const response = await api.put(`/${id}`, valeraData);
    return response.data;
  } catch (error) {
    console.error('Ошибка при обновлении Валеры:', error);
    throw error;
  }
};

// Удалить Валеру
export const deleteValera = async (id) => {
  try {
    const response = await api.delete(`/${id}`);
    return response.data;
 } catch (error) {
    console.error('Ошибка при удалении Валеры:', error);
    throw error;
  }
};

// Выполнить действие с Валерой
export const executeAction = async (id, actionName) => {
  try {
    const response = await api.post(`/${id}/actions/${actionName}`);
    return response.data;
  } catch (error) {
    console.error('Ошибка при выполнении действия:', error);
    throw error;
  }
};

// Сбросить параметры Валеры
export const resetValera = async (id) => {
  try {
    const response = await api.post(`/${id}/reset`);
    return response.data;
  } catch (error) {
    console.error('Ошибка при сбросе параметров Валеры:', error);
    throw error;
  }
};
