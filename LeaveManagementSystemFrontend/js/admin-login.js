const labels = document.querySelectorAll(".form-control label");
labels.forEach((label) => {
  label.innerHTML = label.innerText
    .split("")
    .map(
      (letter, idx) =>
        `<span style="transition-delay:${idx * 50}ms">${letter}</span>`
    )
    .join("");
});

document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("adminLoginForm");
  if (form) {
    form.addEventListener("submit", async function (e) {
      e.preventDefault();

      const email = document.getElementById("email").value.trim();
      const password = document.getElementById("password").value.trim();

      try {
        const response = await fetch("http://localhost:5000/api/Auth/login", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ email, password }),
        });

        if (!response.ok) {
          throw new Error("Login failed");
        }

        const data = await response.json();

        if (!data.token || !data.role) {
          throw new Error("Token or role not found in response");
        }

        localStorage.setItem("jwtToken", data.token);
        localStorage.setItem("email", data.email);
        localStorage.setItem("role", data.role);
        localStorage.setItem("isAdmin", data.role === "Admin");

        alert("Login successful!");
        window.location.href = "adminLeave.html";
      } catch (error) {
        console.error("Login error:", error);
        alert(
          "Login failed. Please check your credentials or try again later."
        );
      }
    });
  }
});
