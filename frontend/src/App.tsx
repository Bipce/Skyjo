import { Route, Routes } from "react-router";
import GameView from "./pages/GameView.tsx";

const App = () => {
  return (
    <>
      <Routes>
        <Route path="/" element={<GameView />} />
      </Routes>
    </>
  );
};

export default App;
