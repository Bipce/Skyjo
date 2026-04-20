interface Props {
  className?: string;
}

const Separation = ({ className }: Props) => {
  return <div className={`absolute top-0 h-full w-px border border-zinc-500/80 ${className}`}></div>;
};

export default Separation;
