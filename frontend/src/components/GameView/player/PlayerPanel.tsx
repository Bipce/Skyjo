import ScorePanel from "../shared/ScorePanel.tsx";
import CardGridWrapper from "../shared/CardGridWrapper.tsx";

const PlayerPanel = () => {
  const cards = [5, 1, 2, "?", 0, "?", 12, -1, 2, 3, 1, 1];

  return (
    <section className="flex h-full w-full justify-center">
      <div className="flex h-full w-2/3 items-center justify-around gap-10 overflow-hidden rounded-xl bg-zinc-950 p-8 shadow">
        <CardGridWrapper cards={cards} className="w-full gap-10 text-5xl" cardClassName="text-xl" />
        <ScorePanel />
      </div>
    </section>
  );
};

export default PlayerPanel;
