document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".editable").forEach(function (element) {
        // Create edit button
        let editBtn = document.createElement("button");
        editBtn.textContent = "ویرایش";
        editBtn.classList.add("btn", "btn-sm", "btn-warning", "edit-btn");
        editBtn.style.zIndex = "9999"; // Ensure it's on top

        // Special case for images: Place button above the image
        if (element.tagName.toLowerCase() === "img") {
            editBtn.textContent = "عکس";
            editBtn.style.position = "absolute";
            editBtn.style.top = "10px";
            editBtn.style.left = "10px";
            element.parentElement.appendChild(editBtn); // Append inside the container
        } else {
            element.after(editBtn); // For text elements, place button next to it
        }


        // Add click event to open modal
        editBtn.addEventListener("click", function () {
            let elementId = element.id;
            let parts = elementId.split("-");
            let prefix = parts[0]; // Get "text" or "img"

            if (prefix === "text") {
                document.getElementById("textElementId").value = elementId; // Pass to text modal
                new bootstrap.Modal(document.getElementById("basicModal-text")).show();
            }
            else if (prefix === "img") {
                document.getElementById("imageElementId").value = elementId; // Pass to image modal
                new bootstrap.Modal(document.getElementById("basicModal-img")).show();
            }
        });
    });
});
$(document).ready(function () {
    $("#editContextForm").on('submit', function (e) {
        e.preventDefault();
        let subbtn = $("#editSiteContext");
        let formData = new FormData(this);
        subbtn.prop("disabled", true).text("در حال ارسال...");
        $.ajax({
            url: "/Component/EditContext",
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.success) {
                    swal.fire({
                        icon: "success",
                        title: "عملیات موفق!",
                        text: res.message,
                        confirmButtonText: "باشه"
                    }).then(() => {
                        location.reload();
                    });
                }
                else {
                    swal.fire({
                        icon: "Error",
                        title: "عملیات ناموفق!",
                        text: res.message,
                        confirmButtonText: "باشه"
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
                swal.fire({
                    icon: "Error",
                    title: "عملیات ناموفق!",
                    text: "خطا از سمت سرور! لطفا در زمانی دیگر امتحان کنید.",
                    confirmButtonText: "باشه"
                });
            },
            complete: function () {
                subbtn.prop("disabled", false).text("ذخیره اطلاعات");
            }
        });
    });
});
$(document).ready(function () {
    $("#ChangeImage").on('submit', function (e) {
        e.preventDefault();
        let formData = new FormData(this);
        $.ajax({
            url: "/Component/SetImage",
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.success) {
                    swal.fire({
                        icon: "success",
                        title: "عملیات موفق!",
                        text: "عکس موردنظر با موفقیت آپلود شد!",
                        confirmButtonText: "باشه"
                    }).then(() => {
                        location.reload();
                    });
                }
                else {
                    swal.fire({
                        icon: "Error",
                        title: "عملیات ناموفق!",
                        text: "آپلود عکس ناموفق بود! لطفا دوباره تلاش کنید.",
                        confirmButtonText: "باشه"
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
                swal.fire({
                    icon: "Error",
                    title: "عملیات ناموفق!",
                    text: "خطا از سمت سرور! لطفا در زمانی دیگر امتحان کنید.",
                    confirmButtonText: "باشه"
                });
            }
        });
    });
});



