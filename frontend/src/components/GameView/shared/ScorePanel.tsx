import React from "react";

const ScorePanel = () => {
  return (
    <aside className="w-32 shrink-0 text-lg">
      <p className="mb-5 font-bold">Pseudo</p>

      <div className="flex flex-col gap-2">
        <h2 className="text-lg font-bold">Scores :</h2>
        <p className="text-lg">Partie : 12</p>
        <p className="text-lg">Global : 25/100</p>
      </div>
    </aside>
  );
};

export default ScorePanel;
