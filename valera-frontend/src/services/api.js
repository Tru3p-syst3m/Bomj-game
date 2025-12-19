import axios from 'axios';

const API_BASE_URL = 'http://localhost:5247/api'; // URL вашего backend API

const api = axios.create({
  baseURL: API_BASE_URL,
});

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

// Создать нового Валеру
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
