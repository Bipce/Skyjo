import { Trophy, Hourglass } from "lucide-react";
import IconWrapper from "../../../ui/IconWrapper.tsx";

interface Props {
  username: string;
}

const ScorePanel = ({ username }: Props) => {
  return (
    <aside className="w-32 shrink-0 text-lg">
      <p className="mb-8 text-2xl font-bold">{username}</p>

      <div className="flex flex-col gap-3">
        <IconWrapper>
          <Hourglass className="text-purple-400" /> : 12
        </IconWrapper>

        <IconWrapper>
          <Trophy className="text-yellow-400" /> : 25 / 100
        </IconWrapper>
      </div>
    </aside>
  );
};

export default ScorePanel;
