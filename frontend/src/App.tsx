import { Route, Routes } from "react-router";
import Game from "./pages/Game.tsx";

function App() {
  return (
    <>
      <Routes>
        <Route path="/" element={<Game />} />
      </Routes>
    </>
  );
}

export default App;
