import './App.css'
import MePanel from './Components/MePanel'
import AuthPanel from './Components/AuthPanel'
import { jwtDecode } from "jwt-decode";
import NavigationPanel from "./Components/NavigationMenu"
import AttributePanel from "./Components/AttributePanel"
import { TooltipProvider } from "./Components/ui/tooltip"
import { Routes, Route, useLocation} from 'react-router-dom';
function App() {

  function defineRole(){
    try{
      let token = localStorage.getItem("token");
      const decoded = jwtDecode(token);

      const currentTime = Date.now();
        if (decoded.exp && decoded.exp * 1000 < currentTime) {
          return "unauthorized"; 
        }
      return decoded.role || "unauthorized";
    }
    catch(ex){
      console.error(ex);
      return "unauthorized";
    }
  }
  const location = useLocation();
  const paddingTop = location.pathname == "/auth" ? "0" : "10";
  return (
    <TooltipProvider delayDuration={300}> 
      <div className="min-h-screen flex flex-col">  
      { location.pathname != "/auth" ? 
      <NavigationPanel role={defineRole()} />
      : null}
      <main className={`flex-1 w-full ${
        location.pathname === "/auth" ? "pt-0" : "pt-10"
      }`}>
        <Routes>
          
          <Route path="/" element={<div>Main page</div>} />

          <Route path="/profile" element={<MePanel />} />

          <Route path="/auth" element={<AuthPanel />} />

          <Route path="/attribute" element={<AttributePanel />}/>
        </Routes>
      </main>

    </div>
    </TooltipProvider>
  )
}

export default App
