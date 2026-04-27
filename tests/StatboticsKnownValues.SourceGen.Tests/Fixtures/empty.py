from sqlalchemy.orm import mapped_column


class EmptyORM(Base, ModelORM):
    """Degenerate fixture: ORM class with no mapped_column declarations.

    Used to verify the generator emits STATBOT001 when an expected ORM
    file produces zero columns.
    """

    __tablename__ = "empty"
    pass
