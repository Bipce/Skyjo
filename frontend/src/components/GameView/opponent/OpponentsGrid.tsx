import React, { type ReactNode } from "react";

interface Props {
  children: ReactNode;
}

const OpponentsGrid = ({ children }: Props) => {
  const count = React.Children.count(children);

  return (
    <section className="w-full">
      <div
        className={`mx-auto grid w-full gap-4 ${
          count === 1 ? "max-w-2xl grid-cols-1" : count === 2 ? "max-w-4xl grid-cols-2" : "grid-cols-3"
        }`}
      >
        {children}
      </div>
    </section>
  );
};

export default OpponentsGrid;
