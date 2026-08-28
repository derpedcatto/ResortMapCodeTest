import type { ReactNode } from "react";
import classNames from "classnames";
import styles from "./BookingLayout.module.scss";

type BookingLayoutProps = {
  map: ReactNode;
  modal?: ReactNode;
  title?: string;
  className?: string;
};

export function BookingLayout({
  map,
  modal,
  title,
  className,
}: BookingLayoutProps) {
  return (
    <div className={classNames(styles.layout, className)}>
      <header className={styles.header}>
        <h1 className={styles.title}>{title}</h1>
      </header>

      <main className={styles.main}>{map}</main>

      {modal}
    </div>
  );
}
