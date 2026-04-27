from sqlalchemy.orm import mapped_column

from src.db.models.types import MI, MOI, MS


class MatchORM(Base, ModelORM):
    """Inline comments and whole-line comments interspersed with declarations."""

    # whole-line comment ahead of declarations should be ignored
    match_key: MS = mapped_column(String(20), index=True)
    red_score: MOI = mapped_column(Integer, nullable=True)  # placeholder for backend API
    # commented_out: MS = mapped_column(String(10))
    blue_score: MOI = mapped_column(Integer, nullable=True)
    time: MI = mapped_column(Integer)
