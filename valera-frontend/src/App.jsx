import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import ValeraList from './components/ValeraList/ValeraList';
import ValeraStats from './components/ValeraStats/ValeraStats';
import './App.css';

function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<ValeraList />} />
          <Route path="/valera/:id" element={<ValeraStats />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
