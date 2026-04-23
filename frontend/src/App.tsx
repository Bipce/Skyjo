import { useEffect } from "react";
import { Route, Routes } from "react-router";
import GameView from "./pages/GameView.tsx";
import { useGameStore } from "./store/gameStore.ts";

const App = () => {
  const bindWindowCallbacks = useGameStore(s => s.bindWindowCallbacks);

  useEffect(() => {
    bindWindowCallbacks();
  }, [bindWindowCallbacks]);

  return (
    <>
      <Routes>
        <Route path="/" element={<GameView />} />
      </Routes>
    </>
  );
};

export default App;
