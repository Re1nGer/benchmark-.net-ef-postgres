CREATE TABLE "Authors" (
                           "Id" SERIAL PRIMARY KEY,
                           "Name" VARCHAR(255) NOT NULL
);

CREATE TABLE "Achievements" (
                                "Id" SERIAL PRIMARY KEY,
                                "Title" VARCHAR(255) NOT NULL,
                                "AuthorId" INTEGER NOT NULL,
                                CONSTRAINT "FK_Achievements_Author"
                                    FOREIGN KEY ("AuthorId")
                                        REFERENCES "Authors"("Id")
                                        ON DELETE CASCADE
);

CREATE TABLE "Books" (
                         "Id" SERIAL PRIMARY KEY,
                         "Title" VARCHAR(255) NOT NULL,
                         "AuthorId" INTEGER NOT NULL,
                         CONSTRAINT "FK_Books_Author"
                             FOREIGN KEY ("AuthorId")
                                 REFERENCES "Authors"("Id")
                                 ON DELETE CASCADE
);

CREATE TABLE "Chapters" (
                            "Id" SERIAL PRIMARY KEY,
                            "Title" VARCHAR(255) NOT NULL,
                            "BookId" INTEGER NOT NULL,
                            CONSTRAINT "FK_Chapters_Book"
                                FOREIGN KEY ("BookId")
                                    REFERENCES "Books"("Id")
                                    ON DELETE CASCADE
);

CREATE TABLE "Reviews" (
                           "Id" SERIAL PRIMARY KEY,
                           "Content" TEXT NOT NULL,
                           "BookId" INTEGER NOT NULL,
                           CONSTRAINT "FK_Reviews_Book"
                               FOREIGN KEY ("BookId")
                                   REFERENCES "Books"("Id")
                                   ON DELETE CASCADE
);

CREATE TABLE "Comments" (
                            "Id" SERIAL PRIMARY KEY,
                            "Content" TEXT NOT NULL,
                            "ChapterId" INTEGER NOT NULL,
                            CONSTRAINT "FK_Comments_Chapter"
                                FOREIGN KEY ("ChapterId")
                                    REFERENCES "Chapters"("Id")
                                    ON DELETE CASCADE
);

CREATE TABLE "ReviewReactions" (
                                   "Id" SERIAL PRIMARY KEY,
                                   "Type" VARCHAR(50) NOT NULL,
                                   "ReviewId" INTEGER NOT NULL,
                                   CONSTRAINT "FK_ReviewReactions_Review"
                                       FOREIGN KEY ("ReviewId")
                                           REFERENCES "Reviews"("Id")
                                           ON DELETE CASCADE,
                                   CONSTRAINT "Check_ReactionType"
                                       CHECK ("Type" IN ('Like', 'Love', 'Wow'))
);

CREATE TABLE "CommentReactions" (
                                    "Id" SERIAL PRIMARY KEY,
                                    "Type" VARCHAR(50) NOT NULL,
                                    "CommentId" INTEGER NOT NULL,
                                    CONSTRAINT "FK_CommentReactions_Comment"
                                        FOREIGN KEY ("CommentId")
                                            REFERENCES "Comments"("Id")
                                            ON DELETE CASCADE,
                                    CONSTRAINT "Check_ReactionType"
                                        CHECK ("Type" IN ('Like', 'Love', 'Wow'))
);
