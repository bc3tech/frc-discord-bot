from sqlalchemy import Boolean, Float, Integer, String
from sqlalchemy.orm import mapped_column

from src.db.models.types import MF, MI, MOI, MS


class TeamORM(Base, ModelORM):
    """Minimal happy-path fixture: 1 ORM class, simple alias-typed columns."""

    __tablename__ = "teams"
    team: MI = mapped_column(Integer, index=True)
    name: MS = mapped_column(String(100))
    country: MS = mapped_column(String(30))
    rookie_year: MOI = mapped_column(Integer, nullable=True)
    epa: MF = mapped_column(Float, default=0)
