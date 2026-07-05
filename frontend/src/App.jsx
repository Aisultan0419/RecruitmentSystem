import './App.css'
import MePanel from './Components/MePanel'
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import AuthPanel from './Components/AuthPanel'
function App() {
  return (
    <>
      <BrowserRouter>
      <nav>
        <Link to="/">Me panel</Link> |{" "}
        <Link to="/about">Registration</Link>
      </nav>

      <Routes>
        <Route path="/" element={<MePanel />} />
        <Route path="/about" element={<AuthPanel />} />
      </Routes>
    </BrowserRouter>
    </>
  )
}

export default App
