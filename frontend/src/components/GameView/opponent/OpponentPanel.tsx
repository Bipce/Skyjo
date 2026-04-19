import ScorePanel from "../shared/ScorePanel.tsx";
import CardGridWrapper from "../shared/CardGridWrapper.tsx";

const OpponentPanel = () => {
  const carts = [5, 1, 2, "?", 2, "?", 5, -1, 2, 3, 1, 1];

  return (
    <div className="mx-auto flex w-full max-w-md items-center justify-between rounded-xl bg-zinc-950 p-5 shadow">
      <CardGridWrapper cards={carts} className="w-1/2 gap-4 text-3xl" cardClassName="text-[9px]" />
      <ScorePanel />
    </div>
  );
};

export default OpponentPanel;
