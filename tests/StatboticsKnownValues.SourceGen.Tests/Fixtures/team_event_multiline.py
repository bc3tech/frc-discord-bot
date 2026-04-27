from sqlalchemy import Enum
from sqlalchemy.orm import Mapped, mapped_column

from src.db.models.types import MB, MI, values_callable
from src.types.enums import EventType


class TeamEventORM(Base, ModelORM):
    """Multi-line mapped_column with `Mapped[EnumType]` annotation."""

    team: MI = mapped_column(Integer, index=True)
    type: Mapped[EventType] = mapped_column(
        Enum(EventType, values_callable=values_callable)
    )
    week: MI = mapped_column(Integer)
    first_event: MB = mapped_column(Boolean)
