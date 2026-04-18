import React, { type ReactNode } from "react";

interface Props {
  children?: ReactNode;
}

const OpponentsGrid = ({ children }: Props) => {
  const opponent = React.Children.count(children);

  return (
    <section className="flex w-full items-center">
      <div
        className={`mx-auto grid w-full gap-4 ${
          opponent === 1 ? "max-w-2xl grid-cols-1" : opponent === 2 ? "max-w-4xl grid-cols-2" : "grid-cols-3"
        }`}
      >
        {children}
      </div>
    </section>
  );
};

export default OpponentsGrid;
