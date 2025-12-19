import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getValeraById, executeAction, resetValera } from '../../services/api';
import './ValeraStats.css';

const ValeraStats = () => {
    const { id } = useParams();
    const [valera, setValera] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchValera();
    }, [id]);

    const fetchValera = async () => {
        try {
            setIsLoading(true);
            const data = await getValeraById(parseInt(id));
            setValera(data);
            setError(null);
        } catch (err) {
            setError('Ошибка при загрузке данных Валеры');
            console.error(err);
        } finally {
            setIsLoading(false);
        }
    };

    const handleAction = async (actionName) => {
        try {
            await executeAction(parseInt(id), actionName);
            // Обновляем данные после действия
            fetchValera();
        } catch (err) {
            setError('Ошибка при выполнении действия');
            console.error(err);
        }
    };

    const handleReset = async () => {
        try {
            await resetValera(parseInt(id));
            // Обновляем данные после сброса
            fetchValera();
        } catch (err) {
            setError('Ошибка при сбросе параметров');
            console.error(err);
        }
    };

    if (isLoading) {
        return <div className="loading">Загрузка...</div>;
    }

    if (error) {
        return <div className="error">Ошибка: {error}</div>;
    }

    if (!valera) {
        return <div className="error">Валера не найден</div>;
    }

    return (
        <div className="valera-stats-container">
            <h1>Статистика Валеры #{valera.id}</h1>

            <div className="valera-details">
                <div className="stats-grid">
                    <div className="stat-item">
                        <h3>Здоровье: <span className={valera.health > 50 ? 'good' : valera.health > 20 ? 'medium' : 'bad'}>{valera.health}</span></h3>
                    </div>
                    <div className="stat-item">
                        <h3>Мана: <span className={valera.mana > 70 ? 'good' : valera.mana > 30 ? 'medium' : 'bad'}>{valera.mana}</span></h3>
                    </div>
                    <div className="stat-item">
                        <h3>Жизнерадостность: <span className={valera.cheerfulness > 3 ? 'good' : valera.cheerfulness > -3 ? 'medium' : 'bad'}>{valera.cheerfulness}</span></h3>
                    </div>
                    <div className="stat-item">
                        <h3>Усталость: <span className={valera.fatigue < 30 ? 'good' : valera.fatigue < 70 ? 'medium' : 'bad'}>{valera.fatigue}</span></h3>
                    </div>
                    <div className="stat-item">
                        <h3>Деньги: <span className={valera.money > 100 ? 'good' : valera.money > 50 ? 'medium' : 'bad'}>{valera.money}</span></h3>
                    </div>
                    <div className="stat-item">
                        <h3>Жив: <span className={valera.isAlive ? 'alive' : 'dead'}>{valera.isAlive ? 'Да' : 'Нет'}</span></h3>
                    </div>
                </div>
            </div>

            <div className="actions">
                <h3>Действия</h3>
                <div className="action-buttons">
                    <button
                        onClick={() => handleAction('work')}
                        className="action-btn work"
                        disabled={valera.fatigue >= 10 || valera.mana >= 50}
                    >
                        Пойти на работу
                    </button>
                    <button
                        onClick={() => handleAction('nature')}
                        className="action-btn nature"
                    >
                        Созерцать природу
                    </button>
                    <button
                        onClick={() => handleAction('wine')}
                        className="action-btn wine"
                        disabled={valera.money < 20}
                    >
                        Пить вино и смотреть сериал
                    </button>
                    <button
                        onClick={() => handleAction('bar')}
                        className="action-btn bar"
                        disabled={valera.money < 100}
                    >
                        Сходить в бар
                    </button>
                    <button
                        onClick={() => handleAction('marginals')}
                        className="action-btn marginals"
                        disabled={valera.money < 150}
                    >
                        Выпить с маргинальными личностями
                    </button>
                    <button
                        onClick={() => handleAction('sing')}
                        className="action-btn sing"
                    >
                        Петь в метро
                    </button>
                    <button
                        onClick={() => handleAction('sleep')}
                        className="action-btn sleep"
                        disabled={valera.mana < 30}
                    >
                        Спать
                    </button>
                </div>
            </div>

            <div className="reset-section">
                <button onClick={handleReset} className="reset-btn">Сбросить параметры</button>
            </div>

            <div className="back-link">
                <Link to="/">Вернуться к списку Валер</Link>
            </div>
        </div>
    );
};

export default ValeraStats;
