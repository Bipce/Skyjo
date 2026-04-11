import { useNavigate } from "react-router";

const Game = () => {
  const navigate = useNavigate();

  return (
    <div>
      <button onClick={() => navigate("/")}>Aller a l'accueil</button>
    </div>
  );
};

export default Game;
