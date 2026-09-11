import { describe, it, expect } from "vitest";
import type { HTTPError } from "ky";
import { http } from "./http";

const URL = "http://localhost/test";
const DEFAULT_MESSAGE = `Request failed with status code 400: GET ${URL}`;
const json = (value: unknown) => JSON.stringify(value);

const getError = async (
  body: string,
  contentType = "application/json",
): Promise<HTTPError> => {
  try {
    await http.get(URL, {
      fetch: () =>
        Promise.resolve(
          new Response(body, {
            status: 400,
            headers: { "content-type": contentType },
          }),
        ),
    });
  } catch (error) {
    return error as HTTPError;
  }
  throw new Error("Expected request to fail");
};

describe("http", () => {
  describe("toProblem", () => {
    it("parses valid json object string", async () => {
      const error = await getError(
        json({ detail: "from string" }),
        "text/plain",
      );

      expect(error.data).toBeTypeOf("string");
      expect(error.message).toBe("from string");
    });

    it("returns undefined for invalid json string", async () => {
      const error = await getError("{ _asd", "text/plain");

      expect(error.message).toBe(DEFAULT_MESSAGE);
    });

    it("object input returns without parsing", async () => {
      const error = await getError(json({ detail: "from object" }));

      expect(error.data).toEqual({ detail: "from object" });
      expect(error.message).toBe("from object");
    });

    it.each(["null", "11", json(["detail", "title"])])(
      "returns default message for %s",
      async (body) => {
        const error = await getError(body);

        expect(error.message).toBe(DEFAULT_MESSAGE);
      },
    );
  });

  describe("beforeError", () => {
    it.each([
      [{ detail: "Detail text" }, "Detail text"],
      [{ title: "Title text" }, "Title text"],
    ])("sets message to %s", async (data, expectedMessage) => {
      const error = await getError(json(data));

      expect(error.message).toBe(expectedMessage);
    });

    it("keeps original message when fields are empty", async () => {
      const error = await getError(json({ status: 400 }));

      expect(error.message).toBe(DEFAULT_MESSAGE);
    });

    it("keeps original message when data is unparseable", async () => {
      const error = await getError("<html>Bad Request</html>", "text/html");

      expect(error.message).toBe(DEFAULT_MESSAGE);
    });
  });
});
