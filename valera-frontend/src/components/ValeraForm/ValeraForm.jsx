import React, { useState } from 'react';
import './ValeraForm.css';

const ValeraForm = ({ onSubmit, onCancel }) => {
    const [valeraData, setValeraData] = useState({
        health: 100,
        mana: 0,
        cheerfulness: 0,
        fatigue: 0,
        money: 100
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setValeraData({
            ...valeraData,
            [name]: parseInt(value)
        });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        onSubmit(valeraData);
    };

    return (
        <form onSubmit={handleSubmit} className="valera-form">
            <h3>Создать нового Валеру</h3>
            <div className="form-grid">
                <div className="form-group">
                    <label htmlFor="health">Здоровье:</label>
                    <input
                        type="number"
                        id="health"
                        name="health"
                        value={valeraData.health}
                        onChange={handleChange}
                        min="0"
                        max="100"
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="mana">Мана:</label>
                    <input
                        type="number"
                        id="mana"
                        name="mana"
                        value={valeraData.mana}
                        onChange={handleChange}
                        min="0"
                        max="100"
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="cheerfulness">Жизнерадостность:</label>
                    <input
                        type="number"
                        id="cheerfulness"
                        name="cheerfulness"
                        value={valeraData.cheerfulness}
                        onChange={handleChange}
                        min="-10"
                        max="10"
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="fatigue">Усталость:</label>
                    <input
                        type="number"
                        id="fatigue"
                        name="fatigue"
                        value={valeraData.fatigue}
                        onChange={handleChange}
                        min="0"
                        max="100"
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="money">Деньги:</label>
                    <input
                        type="number"
                        id="money"
                        name="money"
                        value={valeraData.money}
                        onChange={handleChange}
                        min="0"
                        required
                    />
                </div>
            </div>
            <div className="form-actions">
                <button type="submit" className="submit-btn">Создать Валеру</button>
                <button type="button" onClick={onCancel} className="cancel-btn">Отмена</button>
            </div>
        </form>
    );
};

export default ValeraForm;
