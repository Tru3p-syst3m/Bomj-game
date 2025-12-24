import React, { useState } from 'react';
import { login } from '../services/api';
import { Link, useNavigate } from 'react-router-dom';
import './Login.css';

const Login = ({ setIsAuthenticated }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            await login({ email, password });
            // Обновляем состояние аутентификации
            setIsAuthenticated(true);
            // Отправляем кастомное событие для App.jsx
            window.dispatchEvent(new Event('authChange'));
            // После успешного входа перенаправляем на главную страницу
            navigate('/my');
        } catch (err) {
            setError('Неверный email или пароль');
        }
    };

    return (
        <div className="login-container">
            <div className="login-form">
                <h2>Вход</h2>
                {error && <div className="error">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="email">Email:</label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="password">Пароль:</label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="btn-login">Войти</button>
                </form>
                <div className="auth-link">
                    <p>Нет аккаунта? <Link to="/register">Зарегистрироваться</Link></p>
                </div>
            </div>
        </div>
    );
};

export default Login;
