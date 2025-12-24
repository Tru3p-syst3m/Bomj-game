import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { createValera, getAllValeras } from '../../services/api';
import ValeraForm from '../ValeraForm/ValeraForm';
import '../ValeraForm/ValeraForm.css';
import './ValeraList.css';

const ValeraList = ({ onLogout }) => {
    const [valeras, setValeras] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [showForm, setShowForm] = useState(false);
    const [newValera, setNewValera] = useState({
        health: 100,
        mana: 0,
        cheerfulness: 0,
        fatigue: 0,
        money: 100
    });
    const [userRole, setUserRole] = useState('User'); // По умолчанию User

    // Загрузка списка Валер при монтировании компонента
    useEffect(() => {
        fetchValeras();
        // Получаем роль пользователя из токена
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const base64Url = token.split('.')[1];
                const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
                const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
                    return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
                }).join(''));
                const decodedToken = JSON.parse(jsonPayload);
                setUserRole(decodedToken.role || 'User');
            } catch (err) {
                console.error('Ошибка при декодировании токена:', err);
            }
        }
    }, []);

    const fetchValeras = async () => {
        try {
            setIsLoading(true);
            console.log('Запрос на получение Валер...');
            const data = await getAllValeras();
            console.log('Получены данные:', data);
            setValeras(data);
            setError(null);
        } catch (err) {
            console.error('Ошибка при загрузке данных:', err);
            setError('Ошибка при загрузке данных Валер');
            console.error(err);
        } finally {
            setIsLoading(false);
        }
    };

    const handleSearch = (e) => {
        setSearchTerm(e.target.value);
    };

    // Фильтрация Валер поисковому запросу
    const filteredValeras = valeras.filter(valera =>
        valera.id.toString().includes(searchTerm) ||
        valera.health.toString().includes(searchTerm) ||
        valera.mana.toString().includes(searchTerm) ||
        valera.cheerfulness.toString().includes(searchTerm) ||
        valera.fatigue.toString().includes(searchTerm) ||
        valera.money.toString().includes(searchTerm)
    );

    const handleCreateValera = async (e) => {
        e.preventDefault();
        try {
            const token = localStorage.getItem('token');
            let userId = null;

            if (token) {
                try {
                    const base64Url = token.split('.')[1];
                    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
                    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
                        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
                    }).join(''));
                    const decodedToken = JSON.parse(jsonPayload);
                    userId = decodedToken.nameid || decodedToken.sub;
                } catch (err) {
                    console.error('Ошибка при декодировании токена:', err);
                }
            }

            // Отправляйте правильные данные
            const dataToSend = {
                health: newValera.health,
                mana: newValera.mana,
                cheerfulness: newValera.cheerfulness,
                fatigue: newValera.fatigue,
                money: newValera.money,
                userId: userId, // Добавьте userId вместо строки User
                user: {
                    id: userId,
                    username: localStorage.getItem('user')
                }
            };

            console.log('Отправляемые данные:', dataToSend);
            const createdValera = await createValera(newValera);
            setValeras([...valeras, createdValera]);
            setNewValera({
                health: 100,
                mana: 0,
                cheerfulness: 0,
                fatigue: 0,
                money: 100
            });
            setShowForm(false);
        } catch (err) {
            setError('Ошибка при создании Валеры');
            console.error(err);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setNewValera({
            ...newValera,
            [name]: parseInt(value)
        });
    };

    if (isLoading) {
        return <div className="loading">Загрузка...</div>;
    }

    if (error) {
        return <div className="error">Ошибка: {error}</div>;
    }

    return (
        <div className="valera-list-container">
            <div className="header">
                <h1>Список Валер</h1>
                <button onClick={onLogout} className="logout-btn">Выйти</button>
            </div>

            <div className="controls">
                <input
                    type="text"
                    placeholder="Поиск"
                    value={searchTerm}
                    onChange={handleSearch}
                    className="search-input"
                />
                <button
                    onClick={() => setShowForm(!showForm)}
                    className="create-button"
                >
                    {showForm ? 'Отмена' : 'Создать Валеру'}
                </button>
            </div>

            {showForm && (
                <ValeraForm
                    onSubmit={async (valeraData) => {
                        try {
                            const createdValera = await createValera(valeraData);
                            setValeras([...valeras, createdValera]);
                            setShowForm(false);
                        } catch (err) {
                            setError('Ошибка при создании Валеры');
                            console.error(err);
                        }
                    }}
                    onCancel={() => setShowForm(false)}
                />
            )}

            <div className="valera-list">
                {filteredValeras.length === 0 ? (
                    <p>Нет Валер, соответствующих поисковому запросу</p>
                ) : (
                    filteredValeras.map(valera => (
                        <Link to={`/valera/${valera.id}`} key={valera.id} className="valera-item">
                            <div className="valera-card">
                                <h3>Валера #{valera.id}</h3>
                                <div className="valera-stats">
                                    <p>Здоровье: {valera.health}</p>
                                    <p>Мана: {valera.mana}</p>
                                    <p>Жизнерадостность: {valera.cheerfulness}</p>
                                    <p>Усталость: {valera.fatigue}</p>
                                    <p>Деньги: {valera.money}</p>
                                    <p>Жив: {valera.isAlive ? 'Да' : 'Нет'}</p>
                                </div>
                            </div>
                        </Link>
                    ))
                )}
            </div>
        </div>
    );
};

export default ValeraList;
