import { useEffect } from "react";
import { useShallow } from "zustand/react/shallow";
import { Route, Routes } from "react-router";
import GameView from "./pages/GameView.tsx";
import { useGameStore } from "./store/gameStore.ts";

const App = () => {
  const { bindWindowCallbacks, startNetwork } = useGameStore(
    useShallow(s => ({
      bindWindowCallbacks: s.bindWindowCallbacks,
      startNetwork: s.startNetwork,
    })),
  );

  useEffect(() => {
    bindWindowCallbacks();
    startNetwork();
  }, [bindWindowCallbacks, startNetwork]);

  return (
    <>
      <Routes>
        <Route path="/" element={<GameView />} />
      </Routes>
    </>
  );
};

export default App;
